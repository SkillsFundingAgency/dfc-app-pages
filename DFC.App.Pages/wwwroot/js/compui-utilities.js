class CompUiUtilties {
    static isValidDate(d) {
        return d instanceof Date && !isNaN(d);
    }

    static getDaysInMonth(month, year) {
        return new Date(year, month, 0).getDate();
    }

    static stringUtcToDate(d) {
        var parts = d.split('-');
        return new Date(parts[0], parts[1] - 1, parts[2]); 
    }

    static dateToDmyString(d) {
        var dd = d.getDate();
        var mm = d.getMonth() + 1;
        var yyyy = d.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        return dd + '/' + mm + '/' + yyyy;
    }

    static isInt(value) {
        return !isNaN(value) &&
            parseInt(Number(value)) == value &&
            !isNaN(parseInt(value, 10));
    }

    static getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) === 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
}
