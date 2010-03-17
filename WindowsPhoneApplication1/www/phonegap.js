(function() {
	var PhoneGap = function() {
		this.platform = 'Windows Phone 7';
	};
	PhoneGap.prototype = {
		exec:function(command, params) {
			window.external.Notify(command + (params && params.join?':' + params.join('/'):''));
		},
		evaluate:function(data) {
			eval(data);
		}
	};
	window.gap = new PhoneGap();
})();