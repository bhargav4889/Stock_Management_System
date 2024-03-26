
    function updateTime() {
    const now = new Date();
    let hours = now.getHours();
    let minutes = now.getMinutes();
    let seconds = now.getSeconds();
    const ampm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0'+minutes : minutes;
    seconds = seconds < 10 ? '0'+seconds : seconds;

    const strTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;
    document.getElementById('current-time').innerText = strTime;

    setTimeout(updateTime, 1000);
}

    // Initialize the updateTime function when the page loads
    window.onload = function() {
        updateTime();
};
