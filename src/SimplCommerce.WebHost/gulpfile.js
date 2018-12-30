﻿"use strict";

const gulp = require('gulp');
const clean = require('gulp-clean');
const glob = require("glob");
const rimraf = require("rimraf");
const concat = require("gulp-concat");
const cssmin = require("gulp-cssmin");
const uglify = require("gulp-uglify");
const ignore = require('gulp-ignore');
const del = require('del');
const argv = require('yargs').argv;


const configurationName = argv.configurationName || 'Debug';
const targetFramework = argv.targetFramework || 'netcoreapp2.1';

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
    webroot: "./wwwroot/",
    bower: "./bower_components/"
};

const bower = {
    "font-awesome": "components-font-awesome/**/*.{css,ttf,svg,woff,woff2,eot,otf}",
    "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,woff2,eot}",
    "bootstrap-ui-datetime-picker": "bootstrap-ui-datetime-picker/dist/*.{js,tpls.js}",
    "bootstrap-star-rating": "bootstrap-star-rating/**/star-rating.{css,js}",
    "jquery": "jquery/dist/jquery*.{js,map}",
    "jquery-validation": "jquery-validation/dist/*.js",
    "jquery-validation-unobtrusive": "jquery-validation-unobtrusive/*.js",
    "angular": "angular/*.js",
    "angular-i18n": "angular-i18n/angular-locale_*.js",
    "angular-animate": "angular-animate/angular*.js",
    "angular-aria": "angular-aria/angular*.js",
    "angular-material": "angular-material/angular-material*.{js,css}",
    "angular-messages": "angular-messages/angular-messages*.js",
    "angular-ui-router": "angular-ui-router/release/*.js",
    "angular-smart-table": "angular-smart-table/dist/*.js",
    "angular-loading-bar": "angular-loading-bar/build/loading-bar*.{js,css}",
    "angular-summernote": "angular-summernote/dist/*.js",
    "angular-bootstrap": "angular-bootstrap/ui-bootstrap*.{js,css}",
    "angular-ui-tree": "angular-ui-tree/dist/*.{js,css}",
    "angular-bootstrap-colorpicker": "angular-bootstrap-colorpicker/{js,css,img}/*.*",
    "angular-xeditable": "angular-xeditable/dist/{js,css}/*.{js,css}",
    "ng-file-upload": "ng-file-upload/ng-file-upload*.js",
    "summernote": "summernote/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
    "matchHeight": "matchHeight/dist/jquery.matchHeight*.js",
    "toastr": "toastr/toastr*.{js,css}",
    "bootbox": "bootbox/bootbox*.{js,css}",
    "nouislider": "nouislider/distribute/*.{js,css}",
    "wnumb": "wnumb/wNumb.js",
    "moment": "moment/min/moment.min.js",
    "highcharts-ng": "highcharts-ng/dist/*.{js,css}"
};

gulp.task('clean-modules', function () {
    return gulp.src([mPaths.hostModules + '*', mPaths.hostWwwrootModules + '*'], {
        read: false
    })
    .pipe(clean());
});

gulp.task('copy-compiled', ['clean-modules'], function () {
    modules.forEach(function (module) {
        gulp.src(mPaths.devModules + module.fullName + `/bin/${configurationName}/${targetFramework}/**/*.*`)
        .pipe(gulp.dest(mPaths.hostModules + module.fullName + '/bin'));
    });
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

gulp.task("clean:lib", function () {
    for (let desDir in bower) {
        del.sync(paths.lib + desDir);
    }
});

gulp.task("copy-lib", ["clean:lib"], function () {
    const ignoreComponents = ["**/npm.js"];

    for (let desDir in bower) {
        gulp.src(paths.bower + bower[desDir])
            .pipe(ignore.exclude(ignoreComponents))
            .pipe(gulp.dest(paths.lib + desDir));
    }
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

gulp.task("clean", ["clean-modules", "clean:lib"]);

gulp.task('watch', () => {
    gulp.watch([mPaths.devModules + '**/Views/**/*.*', mPaths.devModules + '**/wwwroot/**/*.*'], ['copy-static']);
});

gulp.task('default', ['copy-lib', 'copy-modules']);
