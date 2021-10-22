/*!
 * gulp
 * $ npm install gulp-ruby-sass gulp-autoprefixer gulp-cssnano gulp-jshint gulp-concat gulp-uglify gulp-imagemin gulp-notify gulp-rename gulp-livereload gulp-cache del --save-dev
 */
// Load plugins
var gulp = require('gulp'),
    gutil = require('gulp-util'),
    sass = require('gulp-ruby-sass'),
    autoprefixer = require('gulp-autoprefixer'),
    cssnano = require('gulp-cssnano'),
    concatCss = require('gulp-concat-css'),
    uglify = require('gulp-uglify'),
    imagemin = require('gulp-imagemin'),
    rename = require('gulp-rename'),
    concat = require('gulp-concat'),
    notify = require('gulp-notify'),
    cache = require('gulp-cache'),
    livereload = require('gulp-livereload'),
    del = require('del'),
    replace = require('gulp-replace'),
    gulpCopy = require('gulp-copy'),
    importCss = require('gulp-import-css'),
    size = require('gulp-size'),
    jshint = require('gulp-jshint'),
    ftp = require('vinyl-ftp'),

    path = {
        images: ['./assets/images/' + '**/*', './assets/image-resources/' + '**/*'],
        bootstrap: './assets/bootstrap/',
        cssCustom: './custom/css-custom/',
        helpers: './assets/helpers/',
        elements: './assets/elements/',
        icons: './assets/icons/',
        iconsCustom: './custom/font-awesome-complete/',
        widgets: './assets/widgets/',
        snippets: './assets/snippets/',
        theme: './assets/themes/',
        jscore: './assets/js-core/',
        init: './assets/js-init/',
        initCustom: './custom/js-init-custom/',
        widgetsCustom: './custom/widgets-custom/'
    }

// START - Task Script para crear bundles de proyecto
// Styles Tasks
gulp.task('styles_bootstrap', function() {
    return gulp.src(path.bootstrap + 'css/bootstrap.css')
        .pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_bootstrap.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_custom', function() {
    return gulp.src(path.cssCustom + '*.css')
        .pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_custom.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_helpers', function() {
    return gulp.src(path.helpers + '*.css')
        .pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_helpers.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_elements', function() {
    return gulp.src(path.elements + '*.css')
        .pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_elements.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_icons', function() {
    return gulp.src(path.icons + '**/*')
        .pipe(gulp.dest('production/bundles/icons'))
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_iconsCustom', function() {
    return gulp.src(path.iconsCustom + '**/*')
        .pipe(gulp.dest('production/bundles/icons/font-awesome-complete'))
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_widgets', function() {
    return gulp.src(path.widgets + '**/*.css')
        .pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_widgets.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_snippets', function() {
    return gulp.src(path.snippets + '*.css')
        .pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_snippets.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})
gulp.task('styles_theme', function() {
    return gulp.src([
            path.theme + 'admin/layout.css',
            path.theme + 'admin/menu.css',
            path.theme + 'admin/color-schemes/default.css',
            path.theme + 'components/default.css',
            path.theme + 'components/border-radius.css',
            path.helpers + 'responsive-elements.css',
            path.helpers + 'admin-responsive.css'
        ])
        //.pipe(autoprefixer('last 4 version'))
        .pipe(concatCss('bundle_theme.css'))
        .pipe(cssnano())
        .pipe(gulp.dest('production/bundles/css'))
        .pipe(size())
        .pipe(notify({ message: 'Styles task complete' }))
})

// Scripts Tasks
gulp.task('scripts_js-core', function() {
    return gulp.src(path.jscore + '*.js')
        .pipe(concat('bundle_scripts_js-core.js'))
        .pipe(uglify())
        .pipe(gulp.dest('production/bundles/scripts'))
        .pipe(size())
        .pipe(notify({ message: 'Scripts task complete' }))
})
gulp.task('scripts_bootstrap', function() {
    return gulp.src(path.bootstrap + 'js/bootstrap.js')
        .pipe(concat('bundle_scripts_bootstrap.js'))
        .pipe(uglify())
        .pipe(gulp.dest('production/bundles/scripts'))
        .pipe(size())
        .pipe(notify({ message: 'Scripts task complete' }))
})
gulp.task('scripts_widgets', function() {
    return gulp.src(path.widgets + '**/*.js')
        .pipe(concat('bundle_scripts_widgets.js'))
        .pipe(uglify())
        .pipe(gulp.dest('_production/scripts'))
        .pipe(size())
        .pipe(notify({ message: 'Scripts task complete' }))
})
gulp.task('scripts_init', function() {
    return gulp.src(path.init + '*.js')
        .pipe(concat('bundle_scripts_init.js'))
        .pipe(uglify())
        .pipe(gulp.dest('_production/scripts'))
        .pipe(size())
        .pipe(notify({ message: 'Scripts task complete' }))
})
gulp.task('scripts_theme', function() {
    return gulp.src(path.theme + 'admin/layout.js')
        .pipe(concat('bundle_scripts_theme.js'))
        .pipe(uglify())
        .pipe(gulp.dest('_production/scripts'))
        .pipe(size())
        .pipe(notify({ message: 'Scripts task complete' }))
})
gulp.task('scripts_widgetspecial', function() {
        return gulp.src(path.widgetsspecial + '**')
            .pipe(gulp.dest('_production/scripts/special'))
            .pipe(notify({ message: 'Styles task complete' }))
    })
    // END - Task Script para crear bundles de proyecto

// Default Styles
gulp.task('styles', function() {
    gulp.start(
        'styles_bootstrap',
        'styles_custom',
        'styles_helpers',
        'styles_elements',
        'styles_icons',
        'styles_iconsCustom',
        'styles_widgets',
        'styles_snippets',
        'styles_theme'
    )
})

// Default Scripts
gulp.task('scripts', function() {
    gulp.start(
        'scripts_js-core',
        'scripts_bootstrap',
        'scripts_widgets',
        'scripts_init',
        'scripts_theme',
        'scripts_widgetspecial'
    )
})

// Default Images
gulp.task('images', function() {
    return gulp.src(path.images)
        .pipe(cache(imagemin({ optimizationLevel: 3, progressive: true, interlaced: true })))
        .pipe(gulp.dest('_production/bundles/styles/**'))
        .pipe(notify({ message: 'Images task complete' }))
})

// FTP Upload
gulp.task('deploy', function() {
    var conn = ftp.create({
        host: 'softwares.infodextra.com',
        user: 'Infodextra',
        password: 'A12345',
        parallel: 10,
        log: gutil.log,
        debug: true
    })
    var globs = '_production/**/*'
    return gulp.src(globs, { base: './_production/', buffer: false })
        .pipe(conn.newer('/_Monarch')) // only upload newer files 
        .pipe(conn.dest('/_Monarch'))
})

// Clean
gulp.task('clean', function() {
    return del(['_production/**', '!_production'])
})

// Default task
gulp.task('default', ['clean'], function() {
    gulp.start('move', 'styles', 'scripts', 'images')
})

// Watch
gulp.task('watch', function() {

    // Watch .scss files
    gulp.watch('src/styles/**/*.scss', ['styles'])

    // Watch .js files
    gulp.watch('src/scripts/**/*.js', ['scripts'])

    // Watch image files
    gulp.watch('src/images/**/*', ['images'])

    // Create LiveReload server
    livereload.listen()

    // Watch any files in _production/, reload on change
    gulp.watch(['_production/**']).on('change', livereload.changed)
})