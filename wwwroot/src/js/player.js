// Get references to DOM elements
const videoPlayer = document.getElementById("videoPlayer");
const playPauseBtn = document.getElementById("playPauseBtn");
const playPauseIcon = document.getElementById("playPauseIcon");
const seekBar = document.getElementById("seekBar");
const volumeControl = document.getElementById("volumeControl");
const currentTimeLabel = document.getElementById("currentTime");
const durationLabel = document.getElementById("duration");
const muteBtn = document.getElementById("muteBtn");
const muteIcon = document.getElementById("muteIcon");
const fullscreenBtn = document.getElementById("fullscreenBtn");
const fullscreenIcon = document.getElementById("fullscreenIcon");
const videoList = document.getElementById("videoList");
const videoItems = document.querySelectorAll(".video-item");

// Update duration and current time of the video
videoPlayer.addEventListener('loadedmetadata', () => {
    const duration = formatTime(videoPlayer.duration);
    durationLabel.textContent = duration;
    seekBar.max = videoPlayer.duration;
});

// Update the current time and seek bar as the video plays
videoPlayer.addEventListener('timeupdate', () => {
    const currentTime = videoPlayer.currentTime;
    currentTimeLabel.textContent = formatTime(currentTime);
    seekBar.value = currentTime;
});

// Play/Pause functionality
playPauseBtn.addEventListener('click', () => {
    if (videoPlayer.paused) {
        videoPlayer.play();
        playPauseIcon.classList.remove("fa-play");
        playPauseIcon.classList.add("fa-pause");
    } else {
        videoPlayer.pause();
        playPauseIcon.classList.remove("fa-pause");
        playPauseIcon.classList.add("fa-play");
    }
});

// Seek bar functionality
seekBar.addEventListener('input', () => {
    videoPlayer.currentTime = seekBar.value;
});

// Volume control functionality
volumeControl.addEventListener('input', () => {
    videoPlayer.volume = volumeControl.value / 100;
    if (videoPlayer.volume === 0) {
        muteIcon.classList.remove("fa-volume-up");
        muteIcon.classList.add("fa-volume-mute");
    } else {
        muteIcon.classList.remove("fa-volume-mute");
        muteIcon.classList.add("fa-volume-up");
    }
});

// Mute/Unmute functionality
muteBtn.addEventListener('click', () => {
    if (videoPlayer.volume > 0) {
        videoPlayer.volume = 0;
        volumeControl.value = 0;
        muteIcon.classList.remove("fa-volume-up");
        muteIcon.classList.add("fa-volume-mute");
    } else {
        videoPlayer.volume = volumeControl.value / 100;
        muteIcon.classList.remove("fa-volume-mute");
        muteIcon.classList.add("fa-volume-up");
    }
});

// Fullscreen functionality
fullscreenBtn.addEventListener('click', () => {
    if (!document.fullscreenElement) {
        videoPlayer.requestFullscreen().catch(err => console.log("Fullscreen error: ", err));
        fullscreenIcon.classList.remove("fa-expand");
        fullscreenIcon.classList.add("fa-compress");
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
        fullscreenIcon.classList.remove("fa-compress");
        fullscreenIcon.classList.add("fa-expand");
    }
});

// Change video on clicking a video item
videoItems.forEach(item => {
    item.addEventListener('click', () => {
        const videoSrc = item.getAttribute('data-video');
        videoPlayer.src = `src/${videoSrc}`;
        videoPlayer.play();
        playPauseIcon.classList.remove("fa-play");
        playPauseIcon.classList.add("fa-pause");
    });
});

// Helper function to format time in MM:SS
function formatTime(seconds) {
    const minutes = Math.floor(seconds / 60);
    const secondsRemaining = Math.floor(seconds % 60);
    return `${minutes}:${secondsRemaining < 10 ? '0' : ''}${secondsRemaining}`;
}
