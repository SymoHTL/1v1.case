window.smoothScrollToPosition = function (position) {
    const containers = document.querySelectorAll('.case-container');
    for (let i = 0; i < containers.length; i++) {
        containers[i].scrollLeft = 0;
        smoothScrollTo(containers[i], position, 900);
    }
}


function smoothScrollTo(container, endX, duration) {
    const startX = container.scrollLeft;
    const change = endX - startX;
    const startDate = +new Date();

    // t = current time
    // b = start value
    // c = change in value
    // d = duration
    const easeInOutQuad = (t, b, c, d) => {
        t /= d / 2;
        if (t < 1) return (c / 2) * t * t + b;
        t--;
        return (-c / 2) * (t * (t - 2) - 1) + b;
    };

    const animateScroll = () => {
        const currentDate = +new Date();
        const currentTime = currentDate - startDate;
        container.scrollLeft = easeInOutQuad(currentTime, startX, change, duration);

        if (currentTime < duration) {
            requestAnimationFrame(animateScroll);
        } else {
            container.scrollLeft = endX; // Ensures we end exactly at our desired scroll position
        }
    };

    animateScroll();
}

function renderFrame(canvasId, data) {
    const canvas = document.getElementById(canvasId);
    const context = canvas.getContext('2d');
    const raw = window.atob(data);
    const rawLength = raw.length;
    const array = new Uint8Array(new ArrayBuffer(rawLength));

    for (let i = 0; i < rawLength; i++) {
        array[i] = raw.charCodeAt(i);
    }

    const blob = new Blob([array], {type: 'image/jpeg'});
    const url = URL.createObjectURL(blob);
    const image = new Image();
    image.onload = function () {
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.drawImage(image, 0, 0, canvas.width, canvas.height);
        URL.revokeObjectURL(url);
    };
    image.src = url;
}


let canvas, context;
let frameBuffer = [];
const frameRate = 30; // Desired frame rate, e.g., 30 frames per second
const frameRenderInterval = 1000 / frameRate; // Calculate interval in ms

let renderingStarted = false;
let currentObjectURL = null; // Keep track of the current Object URL
let renderImage = new Image();


window.renderFrame = function (canvasId, data) {
    frameBuffer.push({ canvasId, data });
    if (!canvas) {
        canvas = document.getElementById(canvasId);
        context = canvas.getContext('2d');
    }

    if (!renderingStarted) {
        renderingStarted = true;
        setInterval(renderFromBuffer, frameRenderInterval);
    }
};

window.clearCanvas = function (canvasId) {
    context.clearRect(0, 0, canvas.width, canvas.height);
    console.log(`Canvas ${canvasId} cleared.`);
    canvas = null;
    context = null; 
}

renderImage.onload = function () {
    
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.drawImage(renderImage, 0, 0, canvas.width, canvas.height);
    if (currentObjectURL) {
        URL.revokeObjectURL(currentObjectURL); // Revoke the previous Object URL
    }
};

function renderFromBuffer() {
    if (frameBuffer.length > 0) {
        const frame = frameBuffer.shift();
        const blob = new Blob([frame.data], {type: 'image/jpeg'});
        currentObjectURL = URL.createObjectURL(blob); // Update current Object URL
        renderImage.src = currentObjectURL;
    }
}


let stop = false;
let lastFrameTime = 0;

window.localVideo = function (dotNetReference, elemId, command) {
    console.log(`localVideo called with command: ${command}`);
    const videoElem = document.getElementById(elemId);
    let canvas, context;
    if (!canvas) {
        canvas = document.createElement('canvas');
        context = canvas.getContext('2d');
    }

    if (videoElem) {
        if (command === 'start') {
            stop = false;
            console.log('Attempting to start video capture...');
            navigator.mediaDevices.getUserMedia({video: true})
                .then((stream) => {
                    console.log('Video capture started.');
                    videoElem.srcObject = stream;
                    videoElem.play();
                    captureFrame();
                })
                .catch((err) => {
                    console.error('Error starting video capture: ', err);
                });
        } else {
            stop = true;
            videoElem.pause();
            videoElem.srcObject.getTracks().forEach(track => track.stop());
            console.log('Video capture stopped.');
            canvas = null;
            context = null;
        }
    }

    function captureFrame() {
        if (stop) return;
        const now = performance.now();
        if (videoElem.srcObject && videoElem.readyState === HTMLMediaElement.HAVE_ENOUGH_DATA && performance.now() - lastFrameTime > 1000 / frameRate) {
            canvas.width = videoElem.videoWidth;
            canvas.height = videoElem.videoHeight;
            context.drawImage(videoElem, 0, 0, canvas.width, canvas.height);
            lastFrameTime = now;
            canvas.toBlob(function (blob) {
                if (blob) {
                    let reader = new FileReader();
                    reader.onloadend = function () {
                        dotNetReference.invokeMethodAsync("SendData", new Uint8Array(reader.result));
                    };
                    reader.readAsArrayBuffer(blob);
                }
            }, 'image/jpeg', 0.1);
        }
        requestAnimationFrame(captureFrame);
    }
};

