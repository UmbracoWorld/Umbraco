import gulp from 'gulp';
import dartSass from 'sass';
import gulpSass from 'gulp-sass';
import gulpWebpack from 'gulp-webpack';
import rename from 'gulp-rename';

const sass = gulpSass( dartSass );

const sassOptions = {
    outputStyle: 'expanded', 
    errLogToConsole: true
};

const destinationPath = "../wwwroot/assets";
const scssPath = "./scss";
const jsPath = "./js"

exports.sass = () => (
    gulp.src(scssPath + '/main.scss')
        .pipe(sass())
        .pipe(gulp.dest(destinationPath + "/css"))
);

gulp.task('js', function () {
    return gulp.src(`${jsPath}/main.js`)
        .pipe(gulpWebpack({
            mode: 'production',
            module: {
                rules: [
                    {
                        test: /\.m?js$/,
                        exclude: /(node_modules|bower_components)/,
                        use: {
                            loader: 'babel-loader',
                            options: {
                                presets: ['@babel/preset-env']
                            }
                        }
                    }
                ]
            }
        }))
        .pipe(rename("main.min.js"))
        .pipe(gulp.dest(destinationPath + "/js"))
})

gulp.task('watch', () => {
    gulp.watch(scssPath + '/**/*', gulp.series('sass'));
    gulp.watch(jsPath + "/**/*.js", gulp.series('js'));
});

gulp.task('default', gulp.series('watch'));
