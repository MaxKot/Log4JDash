'use strict';

function suppressDefaults(form) {
    function doSuppress(rootElement, suppressedElements) {
        var children = rootElement.elements;

        for (var i in children) {
            var child = children[i];

            if (child.tagName === 'INPUT' || child.tagName === 'SELECT') {
                var defaultValue = child.getAttribute('data-default');
                var suppressSubmission = defaultValue !== undefined &&
                                            defaultValue !== null &&
                                            child.value === defaultValue &&
                                            !child.disabled;

                if (suppressSubmission) {
                    child.disabled = true;
                    suppressedElements.push(child);
                }
            }

            doSuppress(child, suppressedElements);
        }
    }

    var suppressedElements = [];
    try {
        doSuppress(form, suppressedElements);
    } catch(e) {
        for (var i in suppressedElements) {
            suppressedElements[i].disabled = false;
        }
        throw e;
    }

    var oldHandler = window.onbeforeunload;
    window.onbeforeunload = function () {
        try {
            for (var i in suppressedElements) {
                suppressedElements[i].disabled = false;
            }
        } finally {
            oldHandler.apply(this, arguments);
        }
    }
}
