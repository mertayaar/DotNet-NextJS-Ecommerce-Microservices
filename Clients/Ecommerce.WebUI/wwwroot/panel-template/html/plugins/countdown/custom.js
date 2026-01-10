

(function ($) {
    "use strict";
    var mainApp = {

        main_fun:function() {
            $(window).load(function () {
                $(".loader").fadeOut("slow");
            });
            $(function () {
                $.vegas('slideshow', {
                    backgrounds: [
                      { src: '../plugins/countdown/img/bg-slide-1.jpg', fade: 1000, delay: 9000 }, 
                      { src: '../plugins/countdown/img/bg-slide-2.jpg', fade: 1000, delay: 9000 }, 
                       { src: '../plugins/countdown/img/bg-slide-3.jpg', fade: 1000, delay: 9000 }, 
                     
                    ]
                })('overlay', {
                    
                    src: '../plugins/countdown/vegas/overlays/15.png' 
                });

            });

            $(function () {
                var $header = $("#headLine");
                

                var position = -1;

                !function loop() {
                    position = (position + 1) % header.length;
                    $header
                        .html(header[position])
                        .fadeIn(1000)
                        .delay(1000)
                        .fadeOut(1000, loop);
                }();
            });

        },

        initialization: function () {
            mainApp.main_fun();

        }

    }
    

    $(document).ready(function () {
        mainApp.main_fun();
    });

}(jQuery));