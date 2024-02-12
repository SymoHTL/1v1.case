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


let currentInterval;
window.receiveData = function (canvasId, data) {
    // data is a string[] of base64 encoded jpeg images
    
    if (currentInterval) {
        clearInterval(currentInterval);
    }
    
    currentInterval = setInterval(() => {
        const d = data.shift();
        let dataBytes = Uint8Array.from(atob(d), c => c.charCodeAt(0));
        renderFrame(canvasId, dataBytes);
    }, 1000 / 30);
     
}


function renderFrame(canvasId, data) {
    const canvas = document.getElementById(canvasId);
    const context = canvas.getContext('2d');
    const blob = new Blob([data], {type: 'image/jpeg'});
    const url = URL.createObjectURL(blob);
    const image = new Image();
    image.onload = function () {
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.drawImage(image, 0, 0, canvas.width, canvas.height);
        URL.revokeObjectURL(url);
    };
    image.src = url;
}

window


let sendBuffer = [];
const fps = 30;

window.localVideo = function (dotNetReference, elemId, command) {
    console.log(`localVideo called with command: ${command}`);
    const videoElem = document.getElementById(elemId);
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');

    if (videoElem) {
        if (command === 'start') {
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
            console.log('Attempting to stop video capture...');
            const stream = videoElem.srcObject;
            if (stream) {
                stream.getTracks().forEach(track => track.stop());
                console.log('Video capture stopped.');
            }
        }
    }

    function captureFrame() {
        if (videoElem.srcObject && videoElem.readyState === HTMLMediaElement.HAVE_ENOUGH_DATA) {
            canvas.width = videoElem.videoWidth;
            canvas.height = videoElem.videoHeight;
            context.drawImage(videoElem, 0, 0, canvas.width, canvas.height);
            canvas.toBlob(function (blob) {
                if (blob) {
                    let reader = new FileReader();
                    reader.onloadend = function () {
                        sendBuffer.push(reader.result);
                        console.log(`sendBuffer.length: ${sendBuffer.length}`);
                        if (sendBuffer.length >= fps) {
                            // convert sendBuffer to a base64 string array
                            let strArray = sendBuffer.map(b => {
                                let binary = '';
                                let bytes = new Uint8Array(b);
                                for (let i = 0; i < bytes.byteLength; i++) {
                                    binary += String.fromCharCode(bytes[i]);
                                }
                                return window.btoa(binary);
                            });
                            dotNetReference.invokeMethodAsync('SendData', strArray);
                            sendBuffer = [];
                        }
                    };
                    reader.readAsArrayBuffer(blob);
                }
            }, 'image/jpeg');
        }
        requestAnimationFrame(captureFrame);
    }
};




