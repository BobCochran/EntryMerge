namespace EntryMergeTest
module MergePuddlesTest =

    open EntryMerge.MergePuddle
    open System.Text.RegularExpressions
    open System
    open Fuchu
    open Microsoft.FSharp.Collections
    

    let contains substring (str: String) =
        str.Contains(substring)
    let beautifiedxml =
        """<?xml version="1.0" encoding="UTF-8"?>
<spml root="http://www.signbank.org/signpuddle2.0" type="sgn" puddle="9040" cdt="1434677474" mdt="1434677474" nextid="9041">
    <term><![CDATA[number]]></term>
    <entry id="1" top="1" next="2" prev="2147" usr="rec">
        <term><![CDATA[gloss: NUMBER]]></term>
        <term><![CDATA[lxsg: ase185]]></term>
        <term><![CDATA[ytlink: YrCXc6jDXbMA]]></term>
        <term><![CDATA[slideset0001]]></term>
        <term><![CDATA[glosskey: 101000]]></term>
        <video><![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/rCXc6jDXbMA?autoplay=1&version=3&loop=1&playlist=rCXc6jDXbMA" frameborder="0" allowfullscreen> </iframe> ]]></video>
    </entry>
    <entry id="2" top="1" next="3" prev="1" cdt="1478202854" mdt="1478363258" usr="rec">
        <term>AS17717S17719S38644S20600S24f00M580x575S17717443x453S17719421x434S20600444x426S38644561x542S24f00426x460</term>
        <struc>S17717,H,R,1,1;S17719,H,L,1,1;S38644,L,R,1,1;S20600,A,B,1,1;S24f00,A,B,1,1</struc>
        <term><![CDATA[gloss: NUMBER]]></term>
        <term><![CDATA[lxsg: ase185]]></term>
        <term><![CDATA[ytlink: YZka0ggbWdJQ]]></term>
        <term><![CDATA[slideset0001]]></term>
        <term><![CDATA[glosskey: 101000]]></term>
        <video><![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/Zka0ggbWdJQ?autoplay=1&version=3&loop=1&playlist=Zka0ggbWdJQ" frameborder="0" allowfullscreen> </iframe> ]]></video>
        <src><![CDATA[SignWriting RC 20161105]]></src>
    </entry>
    <entry id="3" top="1" next="4" prev="2" cdt="1473684654" mdt="1478701459" usr="rec">
        <term>AS31410S15810S22520S26f06M541x542S22520509x471S15810514x487S31410482x483S26f06503x502</term>
        <struc>S31410,L,R,1,1;S15810,H,R,1,1;S22520,A,R,1,1;S26f06,A,R,1,1</struc>
        <term><![CDATA[gloss: NUMBER]]></term>
        <term><![CDATA[lxsg: csl205]]></term>
        <term><![CDATA[ytlink: YV0lIfx76SIU]]></term>
        <term><![CDATA[slideset0001]]></term>
        <term><![CDATA[glosskey: 101000]]></term>
        <video><![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/V0lIfx76SIU?autoplay=1&version=3&loop=1&playlist=V0lIfx76SIU" frameborder="0" allowfullscreen> </iframe> ]]></video>
        <src><![CDATA[NR20161013, EditVS20161109]]></src>
    </entry>
</spml>"""
    let cleanedexpected = 
        ["""PUDDLEID 9040 ENDPUDDLEID<entry id="1" top="1" next="2" prev="2147" usr="rec"><term><![CDATA[gloss: NUMBER]]></term><term><![CDATA[lxsg: ase185]]></term><term><![CDATA[ytlink: YrCXc6jDXbMA]]></term><term><![CDATA[slideset0001]]></term><term><![CDATA[glosskey: 101000]]></term><video><![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/rCXc6jDXbMA?autoplay=1&version=3&loop=1&playlist=rCXc6jDXbMA" frameborder="0" allowfullscreen></iframe> ]]></video></entry>""";
"""PUDDLEID 9040 ENDPUDDLEID<entry id="2" top="1" next="3" prev="1" cdt="1478202854" mdt="1478363258" usr="rec"><term>AS17717S17719S38644S20600S24f00M580x575S17717443x453S17719421x434S20600444x426S38644561x542S24f00426x460</term><struc>S17717,H,R,1,1;S17719,H,L,1,1;S38644,L,R,1,1;S20600,A,B,1,1;S24f00,A,B,1,1</struc><term><![CDATA[gloss: NUMBER]]></term><term><![CDATA[lxsg: ase185]]></term><term><![CDATA[ytlink: YZka0ggbWdJQ]]></term><term><![CDATA[slideset0001]]></term><term><![CDATA[glosskey: 101000]]></term><video><![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/Zka0ggbWdJQ?autoplay=1&version=3&loop=1&playlist=Zka0ggbWdJQ" frameborder="0" allowfullscreen></iframe> ]]></video><src><![CDATA[SignWriting RC 20161105]]></src></entry>""";
"""PUDDLEID 9040 ENDPUDDLEID<entry id="3" top="1" next="4" prev="2" cdt="1473684654" mdt="1478701459" usr="rec"><term>AS31410S15810S22520S26f06M541x542S22520509x471S15810514x487S31410482x483S26f06503x502</term><struc>S31410,L,R,1,1;S15810,H,R,1,1;S22520,A,R,1,1;S26f06,A,R,1,1</struc><term><![CDATA[gloss: NUMBER]]></term><term><![CDATA[lxsg: csl205]]></term><term><![CDATA[ytlink: YV0lIfx76SIU]]></term><term><![CDATA[slideset0001]]></term><term><![CDATA[glosskey: 101000]]></term><video><![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/V0lIfx76SIU?autoplay=1&version=3&loop=1&playlist=V0lIfx76SIU" frameborder="0" allowfullscreen></iframe> ]]></video><src><![CDATA[NR20161013, EditVS20161109]]></src></entry>"""]

    [<Tests>] 
    let tests = 
        testList "Puddle Merge test group" [
            testCase "make one line " <| 
                fun _ -> Assert.Equal("number of lines 1", false,  ["1"; "2";"3";"n"]|> makeoneline |> (fun str -> contains "\r" str ||  contains "\n" str))
            testCase "join tags " <| 
                fun _ -> Assert.Equal("no whitespace between tags", false, "<entry>   <other>" |> jointags |> fun s ->  Regex.IsMatch (s, "[>]\\s+[<]"))
            testCase "join tags beautified xml" <| 
                fun _ -> Assert.Equal("no whitespace between tags", false, beautifiedxml |> jointags |> fun s ->  Regex.IsMatch (s, "[>]\\s+[<]"))
            testCase "puddle id text 1 digit " <| 
                fun _ -> Assert.Equal("create puddle id text", "PUDDLEID 0002 ENDPUDDLEID",  puddleIdText "2" )    
            testCase "puddle id text 2 digit " <| 
                fun _ -> Assert.Equal("create puddle id text", "PUDDLEID 0025 ENDPUDDLEID",  puddleIdText "25" )    
            testCase "puddle id text 3 digit " <| 
                fun _ -> Assert.Equal("create puddle id text", "PUDDLEID 0231 ENDPUDDLEID",  puddleIdText "231" )    
            testCase "puddle id text 4 digit " <| 
                fun _ -> Assert.Equal("create puddle id text", "PUDDLEID 2215 ENDPUDDLEID",  puddleIdText "2215" )    

            testCase "insert newline marker puddleid entry" <| 
                fun _ -> Assert.Equal("insert newline marker entry", """$newline$PUDDLEID 0231 ENDPUDDLEID<entry id="1" top="1" next="2" prev="1026" cdt="1473684654" mdt="1478701459" usr="rec"><term>AS31410S15810S22520S26f06M541x542S22520509x471S15810514x487S31410482x483S26f06503x502</term>""",
                ["""<entry id="1" top="1" next="2" prev="1026" cdt="1473684654" mdt="1478701459" usr="rec">""";"""        <term>AS31410S15810S22520S26f06M541x542S22520509x471S15810514x487S31410482x483S26f06503x502</term>"""] 
                    |> makeoneline |> insertnewlinemarkerpuddleid (puddleIdText "231") )    
            
            testCase "insert newline marker puddleid spml" <| 
                fun _ -> Assert.Equal("insert newline marker spml", """<video> <![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/TlVCs0F69wo?autoplay=1&version=3&loop=1&playlist=TlVCs0F69wo" frameborder="0" allowfullscreen> </iframe> ]]> </video></entry>$newline$</spml>""",
                ["""<video> <![CDATA[<iframe width="560" height="315" src="https://www.youtube.com/embed/TlVCs0F69wo?autoplay=1&version=3&loop=1&playlist=TlVCs0F69wo" frameborder="0" allowfullscreen> </iframe> ]]> </video></entry>"""; """</spml>""";] 
                    |> makeoneline |> insertnewlinemarkerpuddleid (puddleIdText "231") )    
                     
            testCase "split into lines" <| 
                fun _ -> Assert.Equal("split into lines", 3,"$newline$one$newline$two$newline$three" |> splitintolines |> Seq.length )    

            testCase "Keep entries" <| 
                fun _ -> Assert.Equal("split into lines", 2, ["PUDDLEID first line"; "second line"; "PUDDLEID third line" ] |> keepentries |> Seq.length )    
            testCase "Clean entries" <| 
                fun _ -> Assert.Equal("split into lines", cleanedexpected ,   clean "9040" [beautifiedxml] |> Seq.toList ) 
        ]