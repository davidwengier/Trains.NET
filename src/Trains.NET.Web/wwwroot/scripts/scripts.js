



window.canvas = (function () {

    var canvas, context;

    return {
        // Guff:

        init: function (id) {
            canvas = document.getElementById(id);
            context = canvas.getContext('2d');
        },
        getHeight: function () {
            return canvas.clientHeight;
        },
        getWidth: function () {
            return canvas.clientWidth;
        },

        // Drawing:

        clear: function (color) {
            context.fillStyle = color;
            context.fillRect(0, 0, canvas.clientWidth, canvas.clientHeight);
        },

        clipRect: function (l, t, r, b) {
            //context.rect(l, t, r, b);
            //context.clip();
        },

        drawCircle: function (x, y, r, width, color) {
        },

        drawLine: function (x1, y1, x2, y2, width, color) {
            context.strokeStyle = color;
            context.lineWidth = width;

            context.beginPath();
            context.moveTo(x1, y1);
            context.lineTo(x2, y2);
            context.stroke();
        },

        beginPath: function () {
            context.beginPath();
        },

        arcTo: function (radiusX, radiusY, xAxisRotate, arcSize, direction, x, y) {
            
        },

        moveTo: function (x, y) {
            context.moveTo(x, y);
        },

        lineTo: function (x, y) {
            context.lineTo(x, y);
        },

        drawPath: function (width, color) {
            context.strokeStyle = color;
            context.lineWidth = width;
            context.stroke();
        },

        drawRect: function (x, y, w, h, width, color) {
            context.strokeStyle = color;
            context.lineWidth = width;

            context.strokeRect(x, y, w, h);
        },

        drawText: function (text, x, y, size, align, color) {
            context.strokeStyle = color;

            context.textAlign = align;
            context.fillText(text, x, y);
        },

        restore: function () {
            context.restore();
        },

        rotate: function (degrees, x, y) {
            if (x && y) {
                context.translate(x, y);
            }
            context.rotate(degrees);
            if (x && y) {
                context.translate(-x, -y);
            }
        },

        save: function () {
            context.save();
        },

        translate: function (x, y) {
            context.translate(x, y);
        }
    };

})();