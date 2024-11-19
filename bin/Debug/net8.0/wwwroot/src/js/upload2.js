var log_box = document.getElementById("log");
var fileInput = document.getElementById('file'); 
var textInput = document.getElementById('text');
var upload = document.getElementById('upload'); 
var progressBar = document.getElementById('progressBar');
var progressBar2 = document.getElementById('progressBar2');
var download = document.getElementById('download');
var note = document.getElementById('note');
var dropArea = document.getElementById('dropArea');
var logBox = document.getElementById('log');
var dropped = []; // Holds the directory/file tree

var porcent = 0;
var count;
var length;

// The API URL to send files to (update with your dynamic URL)
const apiUrl = '@ip/clownwire/upload/'; // Use your dynamic IP here

// Handle drag over, leave, and drop for the drop area
dropArea.addEventListener('dragover', function(event) {
    event.preventDefault();
    dropArea.classList.add('drag');
});

dropArea.addEventListener('dragleave', function() {
    dropArea.classList.remove('drag');
});

dropArea.addEventListener('drop', function(event) {
    event.preventDefault();
    dropArea.classList.remove('drag');
    const items = event.dataTransfer.items;
    processItems(items); // Process dropped files and directories
});

// Function to process items (files and directories)
function processItems(items) {
    dropped = []; // Reset dropped files array before processing
    for (let i = 0; i < items.length; i++) {
        const entry = items[i].webkitGetAsEntry ? items[i].webkitGetAsEntry() : null;
        if (entry) {
            handleEntry(entry);  // Process each file or directory
        }
    }
}

// Handle a directory or file entry
function handleEntry(entry) {
    if (entry.isDirectory) {
        const dirPath = entry.fullPath || entry.webkitRelativePath;
        const dirItem = { path: dirPath, files: [] };

        const reader = entry.createReader();
        reader.readEntries(entries => {
            entries.forEach(handleEntry); // Recurse into subdirectories
        });
        
        if (dirItem.files.length > 0) {
            dropped.push(dirItem);
        }
    } else if (entry.isFile) {
        entry.file(function(file) {
            const fileItem = { name: file.name, size: file.size, path: file.webkitRelativePath || file.name };
            addFileToTree(fileItem); // Add file to the tree structure
        });
    }
}

// Function to add file to the correct place in the directory tree
function addFileToTree(file) {
    const pathParts = file.path.split('/');
    let currentDir = dropped;

    pathParts.forEach((part, index) => {
        if (index === pathParts.length - 1) {
            currentDir.files = currentDir.files || [];
            currentDir.files.push({ name: part, size: file.size });
        } else {
            let dir = currentDir.find(d => d.path === part);
            if (!dir) {
                dir = { path: part, files: [] };
                currentDir.push(dir); // Add new directory
            }
            currentDir = dir.files; // Traverse to the next level
        }
    });
}

// Function to handle file upload in chunks
async function uploadFileInChunks(file, text, wasDropped) {
    const csize = 50 * 1024 * 1024;
    const chunkSize = csize > file.size?file.size:csize; // 10 MB chunks
    const totalChunks = Math.ceil(file.size / chunkSize);

    progressBar.style.display = 'block'; // Show the progress bar
    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
        porcent = Math.round((chunkIndex + 1) / totalChunks * 100);
        progressBar.value = porcent;
        upload.textContent = `Uploading...: ${porcent}%`;

        const start = chunkIndex * chunkSize;
        const end = Math.min(start + chunkSize, file.size);
        const chunk = file.slice(start, end);

        const formData = new FormData();
        formData.append('fileChunk', chunk, wasDropped ? file.webkitRelativePath : file.name);
        formData.append('text', text);
        formData.append('chunkIndex', chunkIndex);
        formData.append('totalChunks', totalChunks);

        try {
            const response = await fetch(`${apiUrl}/file`, {
                method: 'POST',
                body: formData,
                headers: { 'Accept': 'application/json' },
            });

            if (!response.ok) {
                console.log(`Upload failed: ${response.statusText}`);
                throw new Error(`Upload failed: ${response.statusText}`);
            }

            const result = await response.json();
            console.log(result.message);
        } catch (error) {
            console.error('Error uploading chunk:', error);
            break; // Stop if there's an error
        }
    }

    // After the upload is complete
    upload.textContent = "Upload";
    progressBar.style.display = 'none';
    count++;
    progressBar2.value = Math.round((count) / length * 100);

    if (count === length - 1) {
        progressBar2.style.display = 'none';
        upload.textContent = "Done!!!";
    }
}

// Handle the upload button click
upload.addEventListener('click', () => {
    length = fileInput.files.length;
    count = 0;

    if (dropped.length > 0) {
        processDropUpload(dropped); // Upload dropped files
        return;
    }

    if (length > 1) {
        progressBar2.style.display = 'block'; // Show global progress bar
    }

    for (let f = 0; f < length; f++) {
        let file = fileInput.files[f];
        let text = textInput.value || "not-given"; // Default text if none entered

        if (file) {
            upload.disabled = true;
            download.disabled = true;
            note.disabled = true;
            upload.textContent = "Uploading...";
            log_box.textContent = ""; // Clear logs

            // Start the file upload process
            uploadFileInChunks(file, text, false).finally(() => {
                upload.disabled = false;
                download.disabled = false;
                note.disabled = false;
                upload.textContent = "Upload"; // Reset the button text
            });
        } else {
            console.error('No file selected.');
        }
    }
});

// Function to upload dropped files (directories)
function processDropUpload(files) {
    length = files.length;
    count = 0;

    if (length > 1) {
        progressBar2.style.display = 'block'; // Show global progress bar
    }

    files.forEach(file => {
        let text = textInput.value || "not-given"; // Default text if none entered

        upload.disabled = true;
        download.disabled = true;
        note.disabled = true;
        upload.textContent = "Uploading...";
        log_box.textContent = ""; // Clear logs

        uploadFileInChunks(file, text, true).finally(() => {
            upload.disabled = false;
            download.disabled = false;
            note.disabled = false;
            dropped = [];
            upload.textContent = "Upload"; // Reset button text
        });
    });
}

// Function to prepare the tree structure and upload it
function prepareAndUploadFileTree() {
    let fileTree = buildFileTree(dropped); // Build directory tree from dropped files

    let formData = new FormData();
    formData.append('fileTree', JSON.stringify(fileTree)); // Send file tree as JSON
    formData.append('text', textInput.value || "not-given");

    uploadFileTree(formData); // Upload the file tree
}

// Build a hierarchical file tree from the dropped files
function buildFileTree(files) {
    let fileTree = {};

    files.forEach(file => {
        const pathParts = file.path.split('/');
        let currentDir = fileTree;

        pathParts.forEach((part, index) => {
            if (index === pathParts.length - 1) {
                currentDir.files = currentDir.files || [];
                currentDir.files.push({ name: part, size: file.size });
            } else {
                currentDir[part] = currentDir[part] || {};
                currentDir = currentDir[part];
            }
        });
    });

    return fileTree;
}

// Function to upload the entire file tree to the server
async function uploadFileTree(formData) {
    try {
        const response = await fetch(`${apiUrl}/astree`, {
            method: 'POST',
            body: formData,
            headers: { 'Accept': 'application/json' }
        });

        if (response.ok) {
            const result = await response.json();
            console.log('Upload successful:', result);
            log_box.textContent = "Upload complete!";
        } else {
            console.error('Upload failed:', response.statusText);
            log_box.textContent = `Upload failed: ${response.statusText}`;
        }
    } catch (error) {
        console.error('Error uploading file tree:', error);
        log_box.textContent = "Error uploading file tree.";
    }
}
