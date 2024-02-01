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
        }
        else {
            container.scrollLeft = endX; // Ensures we end exactly at our desired scroll position
        }
    };

    animateScroll();
}