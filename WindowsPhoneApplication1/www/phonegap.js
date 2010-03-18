(function() {
	var PhoneGap = function() {
		this._constructors = [];
		this.available = false;
		this.platform = 'Windows Phone 7';
	};
	PhoneGap.prototype = {
		addConstructor:function(funk) {
			var state = document.readyState;
    		if ( state == 'loaded' || state == 'complete' )
	  		{
		  		funk();
	  		}
    		else
	  		{
        		this._constructors.push(funk);
	  		}
		},
		exec:function(command, params) {
			window.external.Notify(command + (params && params.join?':' + params.join('/'):''));
		},
		evaluate:function(data) {
			eval(data);
		}
	};
	window.gap = new PhoneGap();
	var timer = setInterval(function() {
		var state = document.readyState;
		if ( state == 'loaded' || state == 'complete' )
		{
			clearInterval(timer); // stop looking
			// run our constructors list
			while (window.gap._constructors.length > 0) 
			{
				var constructor = window.gap._constructors.shift();
				try 
				{
					constructor();
				} 
				catch(e) 
				{
					alert('PhoneGap error: Failed to run constructor.');
				}
        	}
			// all constructors run, now fire the deviceready event... but events suck in IE. so, let's just set gap's available member to true.
			// this totally sucks but wtf can we do eh
			window.gap.available = true;
		}
	}, 5);
})();