"use strict";

const gulp = require('gulp');
const clean = require('gulp-clean');
const glob = require("glob");
const rimraf = require("rimraf");
const concat = require("gulp-concat");
const cssmin = require("gulp-cssmin");
const argv = require('yargs').argv;


const configurationName = argv.configurationName || 'Debug';
const targetFramework = argv.targetFramework || 'netcoreapp3.1';

// debugging
console.log(configurationName);
console.log(targetFramework);

const mPaths = {
    devModules: "../Modules/",
    hostModules: "./Modules/",
    hostWwwrootModules: "./wwwroot/modules/"
};

const modules = loadModules();

const paths = {
    webroot: "./wwwroot/"
};

gulp.task('clean-modules', function () {
    return gulp.src([mPaths.hostModules + '*', mPaths.hostWwwrootModules + '*'], {
        read: false
    })
    .pipe(clean());
});

gulp.task('copy-static', ['clean-modules'], function () {
    modules.forEach(function (module) {
        gulp.src([mPaths.devModules + module.fullName + '/Views/**/*.*', mPaths.devModules + module.fullName + '/module.json'], {
            base: module.fullName
        })
        .pipe(gulp.dest(mPaths.hostModules + module.fullName));
        
        gulp.src(mPaths.devModules + module.fullName + '/wwwroot/**/*.*')
        .pipe(gulp.dest(mPaths.hostWwwrootModules + module.name));
    });
    
    gulp.src(mPaths.devModules + 'SimplCommerce.Module.SampleData/SampleContent/**/*.*')
    .pipe(gulp.dest(mPaths.hostModules + 'SimplCommerce.Module.SampleData/SampleContent'));
});

gulp.task('copy-modules', ['clean-modules', 'copy-static']);

function loadModules() {
    let moduleManifestPaths;
    const modules = [];

    moduleManifestPaths = glob.sync(mPaths.devModules + 'SimplCommerce.Module.*/module.json', {});
    moduleManifestPaths.forEach(function (moduleManifestPath) {
        const moduleManifest = require(moduleManifestPath);
        modules.push(moduleManifest);
    });

    return modules;
}

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";
paths.lib = paths.webroot + "lib/";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], {
            base: "."
        })
        .pipe(concat(paths.concatJsDest))
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("clean", ["clean-modules"]);

gulp.task('watch', () => {
    gulp.watch([mPaths.devModules + '**/Views/**/*.*', mPaths.devModules + '**/wwwroot/**/*.*'], ['copy-static']);
});

gulp.task('default', ['copy-modules']);
