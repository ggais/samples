cls
$xx="Count: 200\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n11\r\n10\r\n12\r\n13\r\n14\r\n16\r\n15\r\n17\r\n18\r\n19\r\n20\r\n21\r\n22\r\n23\r\n24\r\n25\r\n26\r\n27\r\n28\r\n29\r\n30\r\n31\r\n32\r\n33\r\n34\r\n35\r\n36\r\n38\r\n37\r\n39\r\n40\r\n41\r\n42\r\n43\r\n44\r\n45\r\n47\r\n46\r\n48\r\n49\r\n50\r\n51\r\n52\r\n53\r\n54\r\n55\r\n56\r\n57\r\n58\r\n59\r\n60\r\n61\r\n62\r\n63\r\n64\r\n65\r\n66\r\n67\r\n68\r\n69\r\n70\r\n71\r\n72\r\n73\r\n74\r\n75\r\n76\r\n77\r\n78\r\n79\r\n80\r\n81\r\n83\r\n82\r\n84\r\n85\r\n86\r\n87\r\n88\r\n89\r\n90\r\n91\r\n92\r\n93\r\n94\r\n95\r\n96\r\n97\r\n98\r\n99\r\n100\r\n101\r\n102\r\n103\r\n104\r\n105\r\n106\r\n107\r\n108\r\n109\r\n110\r\n112\r\n111\r\n113\r\n114\r\n115\r\n116\r\n117\r\n118\r\n119\r\n120\r\n121\r\n122\r\n123\r\n124\r\n126\r\n125\r\n127\r\n128\r\n129\r\n130\r\n131\r\n132\r\n133\r\n134\r\n135\r\n136\r\n137\r\n138\r\n139\r\n140\r\n141\r\n142\r\n143\r\n144\r\n145\r\n146\r\n147\r\n148\r\n149\r\n150\r\n151\r\n152\r\n153\r\n154\r\n155\r\n156\r\n157\r\n158\r\n159\r\n160\r\n161\r\n162\r\n163\r\n164\r\n165\r\n166\r\n167\r\n168\r\n169\r\n170\r\n171\r\n172\r\n173\r\n174\r\n175\r\n176\r\n177\r\n178\r\n179\r\n180\r\n181\r\n182\r\n183\r\n184\r\n185\r\n186\r\n187\r\n188\r\n189\r\n190\r\n191\r\n192\r\n193\r\n194\r\n195\r\n196\r\n197\r\n198\r\n199\r\n200\r\nDone"
$splitChar="\r\n"
$comma = ","

$yy = $xx.Replace($splitChar, $comma).Split($comma)

$current = 0
$previous = 0

foreach($y in $yy){
    $val = [int]::TryParse($y, [REF] $current)

    if ($val -eq $true){
       if ($previous -ne 0 -and $current -ne $previous + 1){
            Write-warning $current
       }
       else{
           $current
       }
       
       $previous = $current
    }
    else {
        
       Write-warning $y
    }

}