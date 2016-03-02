function include(scriptUrl) {
    document.write('<script src="' + scriptUrl + '"></script>');
}

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
};

/* cookie.JS
========================================================*/
include('/js/jquery.cookie.js');


/* DEVICE.JS
========================================================*/
include('/js/device.min.js');

/* Stick up menu
========================================================*/
include('/js/tmstickup.js');
$(window).load(function() { 
  if ($('html').hasClass('desktop')) {
      $('#stuck_container').TMStickUp({
      })
  }  
});

/* Easing library
========================================================*/
include('/js/jquery.easing.1.3.js');


/* ToTop
========================================================*/
include('/js/jquery.ui.totop.js');
$(function () {   
  $().UItoTop({ easingType: 'easeOutQuart' });
});



/* DEVICE.JS AND SMOOTH SCROLLIG
========================================================*/
include('/js/jquery.mousewheel.min.js');
include('/js/jquery.simplr.smoothscroll.min.js');

$(function () { 
  if ($('html').hasClass('desktop')) {
      $.srSmoothscroll({
        step:150,
        speed:800
      });
  }   
});

/* Stellar.js
========================================================*/
include('/js/stellar/jquery.stellar.js');
$(document).ready(function() { 
  if ($('html').hasClass('desktop')) {
      $.stellar({
        horizontalScrolling: false,
        verticalOffset: 20
      });
      

  }  
});

/* Copyright Year
========================================================*/
var currentYear = (new Date).getFullYear();
$(document).ready(function() {
  $("#copyright-year").text( (new Date).getFullYear() );
});


/* Superfish menu
========================================================*/
include('/js/superfish.js');
include('/js/jquery.mobilemenu.js');

/* Owl Carousel
========================================================*/
; (function ($) {
    var o = $('#owl1');
    var o2 = $('#owl2');
    if (o.length > 0) {
        include('/js/owl.carousel.min.js');

        $(document).ready(function () {
            o.owlCarousel({
                responseRefreshRate: 500,
                itemSelector: 'a',
                dots: false,
                nav: true,
                smartSpeed: 700,
                loop: true,
                margin: 30,
                responsive: {
                    0: {
                        items: 1
                    },
                    480: {
                        items: 2
                    },
                    767:{
                        items: 2,
                        margin: 10
                    },
                    768:{
                        items: 3
                    },
                    800:{
                        items: 3,
                        margin: 10
                    },
                    980:{
                        items: 4,
                        margin: 10
                    },
                    1199:{
                        items: 5
                    },
                    1210:{
                        items: 6
                    }
                }
            });
        });
    }

    if (o2.length > 0) {
        include('/js/owl.carousel.min.js');

        $(document).ready(function () {
            o2.owlCarousel({
                responseRefreshRate: 500,
                dots: false,
                nav: true,
                smartSpeed: 700,
                loop:true,
                margin: 30,
                autoWidth:true,
                responsive: {
                    0: {
                        items: 1,
                        autoWidth:false
                    },
                    480: {
                        items: 1,
                        autoWidth:false
                    },
                    768:{
                        items: 1,
                        autoWidth:false
                    },
                    980:{
                        items: 2,
                        margin: 40
                    },
                    1199:{
                        items: 2
                    },
                    1210:{
                        items: 3
                    }
                }

            });
        });
    }
})(jQuery);

/* FancyBox 
========================================================*/ 
;(function ($) { 
    var o = $('.thumb'); 
    if (o.length > 0) { 
        include('/js/jquery.fancybox.js'); 
        include('/js/jquery.fancybox-media.js'); 
        include('/js/jquery.fancybox-buttons.js'); 
        $(document).ready(function () { 
            o.fancybox(); 
        }); 
    } 
})(jQuery);


/* Orientation tablet fix
========================================================*/
$(function(){
// IPad/IPhone
	var viewportmeta = document.querySelector && document.querySelector('meta[name="viewport"]'),
	ua = navigator.userAgent,

	gestureStart = function () {viewportmeta.content = "width=device-width, minimum-scale=0.25, maximum-scale=1.6, initial-scale=1.0";},

	scaleFix = function () {
		if (viewportmeta && /iPhone|iPad/.test(ua) && !/Opera Mini/.test(ua)) {
			viewportmeta.content = "width=device-width, minimum-scale=1.0, maximum-scale=1.0";
			document.addEventListener("gesturestart", gestureStart, false);
		}
	};
	
	scaleFix();
	// Menu Android
	if(window.orientation!=undefined){
  var regM = /ipod|ipad|iphone/gi,
   result = ua.match(regM)
  if(!result) {
   $('.sf-menu li').each(function(){
    if($(">ul", this)[0]){
     $(">a", this).toggle(
      function(){
       return false;
      },
      function(){
       window.location.href = $(this).attr("href");
      }
     );
    } 
   })
  }
 }
});
var ua=navigator.userAgent.toLocaleLowerCase(),
 regV = /ipod|ipad|iphone/gi,
 result = ua.match(regV),
 userScale="";
if(!result){
 userScale=",user-scalable=0"
}
document.write('<meta name="viewport" content="width=device-width,initial-scale=1.0'+userScale+'">')

/* Camera
 ========================================================*/
;
(function ($) {
    var o = $('#camera');
    if (o.length > 0) {
        if (!(isIE() && (isIE() > 9))) {
            include('/js/jquery.mobile.customized.min.js');
        }

        include('/js/camera.js');

        $(document).ready(function () {
            o.camera({
                autoAdvance: true,
                height: '32.78125%',
                minHeight: '300px',
                pagination: true,
                thumbnails: false,
                playPause: false,
                hover: false,
                loader: 'none',
                navigation: false,
                navigationHover: false,
                mobileNavHover: false,
                fx: 'simpleFade'
            })
        });
    }
})(jQuery);

/* Google Map
 ========================================================*/
;
(function ($) {
    var o = document.getElementById("google-map");
    if (o) {
        include('//maps.googleapis.com/maps/api/js?v=3.exp&amp;sensor=false');

        $(document).ready(function () {
            var mapOptions = {
                zoom: 14,
                center: new google.maps.LatLng(parseFloat(40.646197), parseFloat(-73.9724068, 14)),
                scrollwheel: false
            }
            new google.maps.Map(o, mapOptions);
        });
    }
})(jQuery);