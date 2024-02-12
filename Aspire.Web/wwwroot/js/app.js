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
    if (currentInterval) clearInterval(currentInterval);

    currentInterval = setInterval(() => {
        const d = data.shift();
        renderFrame(canvasId, d);
    }, 1000 / 30);
}

function renderFrame(canvasId, data) {
    const canvas = document.getElementById(canvasId);
    const context = canvas.getContext('2d');
    const raw = window.atob(data);
    const rawLength = raw.length;
    const array = new Uint8Array(new ArrayBuffer(rawLength));

    for(let i = 0; i < rawLength; i++) {
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
                    const recorder = new MediaRecorder(stream);
                    recorder.ondataavailable = (e) => {
                        sendBuffer.push(e.data);
                        if (sendBuffer.length >= fps / 2) {
                            // get all text from every frame using .text() promise and promise.all

                            Promise.all(sendBuffer.map((blob) => {
                                return blob.text();
                            })).then((textArray) => {
                                dotNetReference.invokeMethodAsync('SendData', textArray);
                            });
                            sendBuffer = [];
                        }
                    };
                    recorder.start(1000 / fps);
                    recorder.onstop = () => {
                        console.log('Recorder stopped.');
                    };
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
};
