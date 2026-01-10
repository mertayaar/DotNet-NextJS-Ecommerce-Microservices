'use strict'

var gulp = require('gulp');
var browserSync = require('browser-sync').create();
var sass = require('gulp-sass');
var del = require('del');
var replace = require('gulp-replace');
var injectPartials = require('gulp-inject-partials');
var inject = require('gulp-inject');
var sourcemaps = require('gulp-sourcemaps');
var concat = require('gulp-concat');
var merge = require('merge-stream');



gulp.task('sass', function () {
    return gulp.src('./scss/style.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({outputStyle: 'expanded'}).on('error', sass.logError))
        .pipe(sourcemaps.write('./maps'))
        .pipe(gulp.dest('./css'))
        .pipe(browserSync.stream());
});


gulp.task('serve', gulp.series('sass', function() {

    browserSync.init({
        port: 3000,
        server: "./",
        ghostMode: false,
        notify: false
    });

    gulp.watch('scss/**/*.scss', gulp.series('sass'));
    gulp.watch('**/*.html').on('change', browserSync.reload);
    gulp.watch('js/**/*.js').on('change', browserSync.reload);

}));




gulp.task('serve:lite', function() {

    browserSync.init({
        server: "./",
        ghostMode: false,
        notify: false
    });

    gulp.watch('**/*.css').on('change', browserSync.reload);
    gulp.watch('**/*.html').on('change', browserSync.reload);
    gulp.watch('js/**/*.js').on('change', browserSync.reload);

});



gulp.task('injectPartial', function () {
  return gulp.src("./**/*.html", { base: "./" })
    .pipe(injectPartials())
    .pipe(gulp.dest("."));
});




gulp.task('injectCommonAssets', function () {
  return gulp.src('./**/*.html')
    .pipe(inject(gulp.src([ 
        './vendors/mdi/css/materialdesignicons.min.css',
        './vendors/css/vendor.bundle.base.css', 
        './vendors/js/vendor.bundle.base.js',
    ], {read: false}), {name: 'base', relative: true}))
    .pipe(inject(gulp.src([
        './css/*.css', 
        './js/off-canvas.js', 
        './js/hoverable-collapse.js',
        './js/template.js',
    ], {read: false}), {relative: true}))
    .pipe(gulp.dest('.'));
});


gulp.task('injectLayoutStyles', function () {
    var verticalLightStream = gulp.src(['./**/**/*.html',
            './index.html'])
        .pipe(inject(gulp.src([
        './css/style.css', 
        ], {read: false}), {relative: true}))
        .pipe(gulp.dest('.'));
    return merge(verticalLightStream);
});


gulp.task('replacePath', function(){
    var replacePath1 = gulp.src(['./pages/*/*.html'], { base: "./" })
        .pipe(replace('="images/', '="../../images/'))
        .pipe(replace('href="pages/', 'href="../../pages/'))
        .pipe(replace('href="index.html"', 'href="../../index.html"'))
        .pipe(replace('href="docs/', 'href="../../docs/'))
        .pipe(gulp.dest('.'));
    var replacePath2 = gulp.src(['./*.html'], { base: "./" })
        .pipe(replace('="images/', '="images/'))
        .pipe(replace('"pages/', '"pages/'))
        .pipe(replace('href="index.html"', 'href="index.html"'))
        .pipe(replace('"docs/', '"docs/'))
        .pipe(gulp.dest('.'));
    return merge(replacePath1, replacePath2);
});


gulp.task('inject', gulp.series('injectPartial' , 'injectCommonAssets' , 'injectLayoutStyles', 'replacePath'));

gulp.task('clean:vendors', function () {
    return del([
      'vendors/**/*'
    ]);
});


gulp.task('buildBaseVendorScripts', function() {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js', 
        './node_modules/popper.js/dist/umd/popper.min.js', 
        './node_modules/bootstrap/dist/js/bootstrap.min.js', 
        './node_modules/perfect-scrollbar/dist/perfect-scrollbar.min.js'
    ])
      .pipe(concat('vendor.bundle.base.js'))
      .pipe(gulp.dest('./vendors/js'));
});


gulp.task('buildBaseVendorStyles', function() {
    return gulp.src(['./node_modules/perfect-scrollbar/css/perfect-scrollbar.css'])
      .pipe(concat('vendor.bundle.base.css'))
      .pipe(gulp.dest('./vendors/css'));
});


gulp.task('copyAddonsScripts', function() {
    var aScript1 = gulp.src(['node_modules/chart.js/dist/Chart.min.js'])
        .pipe(gulp.dest('./vendors/chart.js'));
    return merge(aScript1);
});



gulp.task('copyAddonsStyles', function() {
    var aStyle1 = gulp.src(['./node_modules/@mdi/font/css/materialdesignicons.min.css'])
        .pipe(gulp.dest('./vendors/mdi/css'));
    var aStyle2 = gulp.src(['./node_modules/@mdi/font/fonts/*'])
        .pipe(gulp.dest('./vendors/mdi/fonts'));
    return merge(aStyle1, aStyle2);
});


gulp.task('bundleVendors', gulp.series('clean:vendors', 'buildBaseVendorStyles','buildBaseVendorScripts', 'copyAddonsStyles', 'copyAddonsScripts'));

gulp.task('default', gulp.series('serve'));
