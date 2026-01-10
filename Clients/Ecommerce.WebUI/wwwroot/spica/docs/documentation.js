(function($) {
  'use strict';
  $(function() {
    
    function offsetAnchor() {
        if (location.hash.length !== 0) {
            
            $("html").animate({ scrollTop: $(location.hash).offset().top - 160 }, 500);
        }
    }
    
    
    $(document).on('click', 'a[href^="#"]', function(event) {
        
        
        window.setTimeout(function() {
        offsetAnchor();
        }, 0);
    });
    
    
    window.setTimeout(offsetAnchor, 0);
    });
})(jQuery);