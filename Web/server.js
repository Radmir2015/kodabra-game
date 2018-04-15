var needle = require('needle');
var cheerio = require('cheerio');
var fs = require("fs");
var path = require("path");

var express = require("express");
var app = express();
var bodyParser = require("body-parser");

var urlencodedParser = bodyParser.urlencoded({ extended: false});

app.set("view engine", "ejs");
app.set("views", path.join(__dirname + "/views"));

// var url = "http://kgu.ru/uchenikam/terminy/";

var result = [];

app.get("/", (req, res) => {
    res.render("startForm");
});

app.post("/data", urlencodedParser, (req, resp) => {
    console.log(req.body);
    var options = req.body;
    try {
        needle.get(options.url, function(err, res){
            if (err) {
                resp.render("error");
                return err;
            }
            
            var $ = cheerio.load(res.body);

            if (!options.dict) {
                var elem = $(options.selector); // "td[valign=top] + td > p"
                fs.writeFileSync("./data.txt", elem.text());
            } else {
                var obj = {};

                $(options.selector).each(function() { // "p[style]"
                    var strong = $(this).find("strong").text();
                    var sliceShift = 0;
                    var allText = $(this).text();
                    if (allText.trim().split(" ").length > strong.trim().split(" ").length && allText.trim().split(" ")[strong.trim().split(" ").length].startsWith("(")) {
                        sliceShift = allText.trim().indexOf(").") + 3
                        obj[strong.trim()] = allText.slice(sliceShift);
                    } else {
                        obj[strong.trim()] = allText.slice(strong.length);
                    }
                    // console.log(strong, strong.trim().split(" ").length, $(this).text().split(" ")[strong.trim().split(" ").length]);
                });

                fs.writeFileSync("./dict.json", JSON.stringify(obj));
            }

            console.log("File is written...");
            resp.render("success");
        });
    }
    catch (e) {
        resp.render("error");
    }
});

app.listen(8000, () => console.log("Server is working..."));