var interval = null;
var timeOut = null;
var angle = 0;
var link = "";
var progress = 0;

const songs = document.querySelectorAll('#links_list .links');
const progressElement = document.querySelector('.progress');
const statusElement = document.getElementById('sync_status');
const warningElement = document.getElementById("warning");

const Download = () => {
    warningElement.innerHTML = "Please wait, this could take a while...<br/><br/>Close this if no songs are being downloaded.";
    console.log("Download Started...");
    
    songs.forEach(item => {
        const randomNumber = Math.floor(Math.random() * (5000 - 1000 + 1)) + 1000;  // Random delay between 1000 and 5000 milliseconds
        
        setTimeout(() => {
            console.log("Clicking: ", item);
            item.click();  // Programmatically click the <a> tag
        }, randomNumber);  // Delay the click with the random number
    });
};

const Sync = () => {       
    angle += 45;  // Increase angle by 45 degrees
    const svg = document.getElementById('sync_svg');
    svg.style.transform = `rotate(${angle}deg)`; 

    progress += 1;  // Increment progress
    if (progress == 100) {
        progress = 0;
    }

    progressElement.style.width = progress + '%';  // Update the progress bar width
    statusElement.textContent = progress + '%';  // Update the status text

    if (interval == null) {
        interval = setInterval(() => {
            Sync();
        }, 200);  // Run Sync function at intervals
        Download();  // Trigger the download function
    }
}