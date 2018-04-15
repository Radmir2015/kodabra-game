var needle = require('needle');
var cheerio = require('cheerio');
var fs = require("fs");

var url = "http://www.mathprofi.ru/predely_primery_reshenii.html";

var result = [];

needle.get(url, function(err, res){
    if (err) throw err;
    
    var $ = cheerio.load(res.body);

    var elem = $("td[valign=top] + td > p");

    //console.log(result);

    console.log(elem.length);
    console.log(elem.text().indexOf('\n'));

    fs.writeFileSync("./data.txt", elem.text());

    console.log("File is written...");

});