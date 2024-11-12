let fileList = [];
let totalFiles = 0;
let currentFileIndex = 0;

const uploadBtn = document.getElementById("upload");
const logBox = document.getElementById("log");
const fileInput = document.getElementById("fileInput");
const progressBar = document.getElementById("progressBar");
const progressBar2 = document.getElementById("progressBar2");
const uploadStatus = document.getElementById("uploadStatus");

// This will be replaced dynamically with the correct API URL
const apiUrl =  '@ipaddress/clownwire/upload/upload'; // Replace with your upload API URL

// Update file list when files are selected
function updateFileList() {
    fileList = Array.from(fileInput.files);
    totalFiles = fileList.length;
    renderFileList();
}

// Render the list of files
function renderFileList() {
    const fileListContainer = document.getElementById('fileList');
    fileListContainer.innerHTML = '';

    if (fileList.length === 0) {
        fileListContainer.innerHTML = '<p>No files selected.</p>';
    } else {
        fileList.forEach((file, index) => {
            const fileItem = document.createElement('div');
            fileItem.className = 'file-item';
            let fileIcon = '📄';
            if (file.type.includes('image')) fileIcon = '🖼️';
            else if (file.type.includes('audio')) fileIcon = '🎵';
            else if (file.type.includes('video')) fileIcon = '🎥';
            fileItem.innerHTML = `
                <i>${fileIcon}</i>
                <p>${file.name}</p>
            `;
            fileListContainer.appendChild(fileItem);
        });
    }
}

// Function to log messages in the log box
function log(message) {
    logBox.textContent = message + "\n" + logBox.textContent;
}

// Function to upload file in chunks
async function uploadFileInChunks(file, text) {
    const chunkSize = 10 * 1024 * 1024; // 10 MB chunks
    const totalChunks = Math.ceil(file.size / chunkSize);

    progressBar.style.display = 'block'; // Show progress bar for each file
    uploadStatus.textContent = `Uploading file: ${file.name}`;
    
    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
        const start = chunkIndex * chunkSize;
        const end = Math.min(start + chunkSize, file.size);
        const chunk = file.slice(start, end);

        const formData = new FormData();
        formData.append('fileChunk', chunk, file.name);
        formData.append('text', text); // You can add additional info to the upload
        formData.append('chunkIndex', chunkIndex);
        formData.append('totalChunks', totalChunks);

        const percentage = Math.round((chunkIndex + 1) / totalChunks * 100);
        progressBar.value = percentage;
        uploadStatus.textContent = `Uploading... ${percentage}%`;

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                body: formData,
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error('Upload failed: ' + response.statusText);
            const result = await response.json();
            log(result.message);

        } catch (error) {
            log(`Error uploading chunk: ${error}`);
            break;
        }
    }

    // After upload complete for this file
    log(`File ${file.name} uploaded successfully!`);
    currentFileIndex++;
    updateGlobalProgress();
}

// Function to update the global progress bar
function updateGlobalProgress() {
    const globalProgress = Math.round((currentFileIndex / totalFiles) * 100);
    progressBar2.value = globalProgress;
    uploadStatus.textContent = `Global upload progress: ${globalProgress}%`;

    if (currentFileIndex === totalFiles) {
        progressBar.style.display = 'none';
        progressBar2.style.display = 'none';
        uploadStatus.textContent = 'All files uploaded successfully!';
    }
}

// Start upload for all files
async function startUpload() {
    if (fileList.length === 0) {
        log('No files to upload');
        return;
    }

    currentFileIndex = 0;
    progressBar2.style.display = 'block'; // Show global progress bar
    progressBar2.value = 0;
    progressBar.value = 0;
    
    for (let i = 0; i < fileList.length; i++) {
        const file = fileList[i];
        const text = 'No additional info'; // Use any extra information you want to send
        await uploadFileInChunks(file, text);
    }
}
