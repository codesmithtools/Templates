Ext.ns("Tracker");

/**
* @class Feedback.Utilities
* @classDescription Utility Methods for Feedback
* @author Paul Welter
* @author Eric Smith
* @author Reggie Bradshaw and Tom Dupont
* @version 1.0
*/
Tracker.Utilities = function() {

var jsonDateRe = /\/Date\((\d+)\)\//g;
var jsonDateFormat = 'Y-m-dTH:i:s.u';

return {
        parseJsonDate: function(value) {
            return value ? Date.parseDate(value, jsonDateFormat) : null;
        },
        /**
        * @public
        * @method
        * @returns String
        */
        getJsonDateFormat: function() {
            return jsonDateFormat;
        },
        /**
        * @public
        * @method
        * @param {Date}
        *            value
        * @return {Date} convertDate
        */
        convertDate: function(value) {
            if (value && value.match(jsonDateRe)) {
                return eval(value.replace(jsonDateRe, 'new Date($1)'));
            }
            return null;
        }
    };
} ();
