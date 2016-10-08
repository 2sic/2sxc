// Initial variables, constants, etc.
var gulp = require("gulp"),
    $ = require("gulp-load-plugins")({ lazy: false }),
    packageJSON = require('./package'),
    // would need this to always auto-publish after compile... runSequence = require('run-sequence'),
    jshintConfig = packageJSON.jshintConfig,
    merge = require("merge-stream"),
    js = "js",
    css = "css",
    config = {
        debug: true,
        autostart: true,
        rootDist: "dist/" // "tmp-gulp/dist/"
    };


// register all watches & run them
gulp.task("watch-our-code", function () {
    watchSet(createSetsForOurCode());
});

// test something - add your code here to test it
gulp.task("test-something", function () {

});

//gulp.task("clean-dist", function () {
    // disabled 2016-03-13 to prevent mistakes as gulp doesn't generate everything yet
    //gulp.src(config.rootDist)
    //    .pipe($.clean());
//});


//#region basic functions I'll need a lot
function createConfig(key, tmplSetName, altDistPath, altJsName, libFiles) {
    var cwd = "src/" + key + "/";
    return {
        name: key,
        cwd: cwd,
        dist: altDistPath || config.rootDist + key + "/",
        css: {
            run: true,
            alsoRunMin: true,
            files: [cwd + "**/*.css"],
            libs: [],
            concat: key + ".css"
        },
        js: {
            run: true,
            files: [cwd + "**/*.js", "!" + cwd + "**/*spec.js", "!" + cwd + "**/tests/**"],
            libs: libFiles || [],
            concat: altJsName || key + ".js",
            templates: ["src/" + key + "/**/*.html"],
            templateSetName: tmplSetName,
            autoSort: true,
            alsoRunMin: true
        }
    }
}

// package a JS set
function packageJs(set) {
    if (config.debug) console.log("bundling start: " + set.name);

    var js = gulp.src(set.js.files);
    if (set.js.autoSort)
        js = js.pipe($.sort());
    js = js.pipe($.jshint(jshintConfig))
        .pipe($.jshint.reporter("jshint-stylish"))
        //.pipe($.jshint.reporter('fail'))
        .pipe($.ngAnnotate());

    var tmpl = set.js.templates ? gulp.src(set.js.templates)
        .pipe($.sort())
        //.pipe($.htmlmin({ collapseWhitespace: true }))
        .pipe($.angularTemplatecache("templates.js", { // set.js.templateSetName + ".js", { //"templates.js", {
            standalone: true,
            module: set.js.templateSetName // "eavTemplates"
        })) : null;

    var libs = gulp.src(set.js.libs);

    var prelib = merge(js, tmpl);
    if (set.js.autoSort)
        prelib = prelib.pipe($.sort());

    var result = merge(libs, prelib);
    if (set.js.autoSort)
        result = result.pipe($.sort());

    result = result.pipe($.concat(set.js.concat))
        .pipe(gulp.dest(set.dist))
        .pipe($.rename({ extname: ".min.js" }));
    // 2016-04-23 2dm had to disable source-maps for now, something is buggy inside
    // 2016-09-07 2dm re-enabled it, seems to work now...
    // 2016-09-08 2rm had to disable it again, sourcmap generator throws an error
    if (set.js.alsoRunMin)
        result = result
                .pipe($.sourcemaps.init({ loadMaps: true }))
                .pipe($.uglify())
                .on("error", $.util.log)
             .pipe($.sourcemaps.write("./"))
            .pipe(gulp.dest(set.dist));

    if (config.debug) console.log($.util.colors.cyan("bundling done: " + set.name));

    return result;
}

// package a set of CSS
function packageCss(set) {
    if (config.debug) console.log("css packaging start: " + set.name);

    var result = gulp.src(set.css.files)
        .pipe($.sort());
    // lint the css - not enabled right now, too many fix-suggestions
    //.pipe($.csslint())
    //.pipe($.csslint.reporter())
    var libs = gulp.src(set.css.libs);  // don't sort libs

    result = merge(result, libs)

        // concat & save concat-only (for debugging)
        .pipe($.concat(set.css.concat))
        .pipe(gulp.dest(set.dist));

    if (set.css.alsoRunMin)
        result
            // minify and save
            .pipe($.rename({ extname: ".min.css" }))
            .pipe($.sourcemaps.init())
            .pipe($.cleanCss({ compatibility: "*", processImportFrom: ['!fonts.googleapis.com'] /* ie9 compatibility */ }))
            .pipe($.sourcemaps.write("./"))
            .pipe(gulp.dest(set.dist));
    ;
    if (config.debug) console.log($.util.colors.cyan("css packaging done: " + set.name));
    return result;
}

// assemble a function which will call the desired set - this is a helper for the watch-sequence. 
function createWatchCallback(set, part) {
    if (config.debug) console.log("creating watcher callback for " + set.name);
    var run = function (event) {
        if (config.debug) console.log("File " + event.path + " was " + event.type + ", running tasks on set " + set.name);
        var call = (part === js) ? packageJs : packageCss;
        call(set);
        console.log("finished '" + set.name + "'" + new Date());
    }
    if (config.autostart)
        run({ path: "[none]", type: "autostart" });
    return run;
}


//#endregion



/// create watch-sets for all our code blocks
function createSetsForOurCode() {
    var sets = [];
    // setup admin, exclude pipeline css (later also exclude pipeline js)
    var admin = createConfig("sxc-admin", "sxcTemplates");
    //admin.css.files.push("!" + admin.cwd + "**/pipeline*.css");
    sets.push(admin);

    // setup edit & extended
    var edit = createConfig("sxc-edit", "sxcTemplates");
    sets.push(edit);

    sets = [];
    // setup inpage stuff
    var inpage = createConfig("inpage", "templates");
    sets.push(inpage);

    // setup inpage dialogs
    var inpDialog = createConfig("inpage-dialogs", "templates", "dist/inpage/");

    sets.push(inpDialog);


    return sets;
}

/// let gulp watch a series of packs
function watchSet(setList) {
    for (var i = 0; i < setList.length; i++) {
        var x = setList[i];
        if (x.js.run) gulp.watch(x.cwd + "**/*", createWatchCallback(x, js));
        if (x.css.run)
            gulp.watch(x.cwd + "**/*", createWatchCallback(x, css));
    }
}
