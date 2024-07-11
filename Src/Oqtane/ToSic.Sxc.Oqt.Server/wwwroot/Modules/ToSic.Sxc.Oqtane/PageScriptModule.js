function reExecuteScripts() {
    // Get all script elements on the page
    const scripts = document.querySelectorAll('script');

    // List of scripts to skip
    const skipScripts = ['blazor.web.js', 'app.js', 'loadjs.min.js', 'interop.js', 'Module.js', 'PageScriptModule.js', 'turn-on.js', '2sxc.api', 'inpage'];

    // Function to check if script src contains any of the scrpts to skip
    function shouldSkipScript(src) {
        return src == '' || skipScripts.some(script => src.includes(script));
    }

    // Iterate over each script element
    scripts.forEach((script) => {
        const src = script.getAttribute('src') || '';

        if (shouldSkipScript(src)) {
            console.log(`skipping script: ${(!!src) ? src : script.textContent}`);
            return;
        }

        // Create a new script element
        const newScript = document.createElement('script');

        // Copy attributes from the old script to the new script
        for (const attr of script.attributes) {
            newScript.setAttribute(attr.name, attr.value);
        }

        // Copy the content of the script
        if (script.src) {
            // If the script is external, just set the src attribute
            newScript.src = script.src;
        } else {
            // If the script is inline, copy its content
            newScript.textContent = script.textContent;
        }

        // Replace the old script with the new script
        script.parentNode.replaceChild(newScript, script);

        console.log('re-execute', script.src);
    });

    console.log('All scripts have been re-executed.');
}

// This method is executed after enhanced form calls and once at startup
// Called when the script first gets loaded on the page.
export function onLoad() {
    console.log('Load');
}

// Called when an enhanced page update occurs, plus once immediately after
// the initial load.
export function onUpdate() {
    console.log('Update');
    // reExecuteScripts();
}

// Called when an enhanced page update removes the script from the page.
export function onDispose() {
    console.log('Dispose');
}