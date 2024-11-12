from flask import Flask, Response, request
import os
import subprocess
from flask_cors import CORS

app = Flask(__name__)
CORS(app, resources={r"/*": {"origins": "*"}})  # Enable CORS for all routes

# Path to your MKV file
MKV_FILE_PATH = 'video.mkv'

@app.route('/stream')
def stream_video():
    # Get the file size of the MKV video
    file_size = os.path.getsize(MKV_FILE_PATH)

    # Get the range from the request headers (e.g., "bytes=0-1023")
    byte_range = request.headers.get('Range', None)
    
    # If the range is requested (e.g., "bytes=0-1023")
    if byte_range:
        byte_range = byte_range.strip().split('=')[1]
        start, end = byte_range.split('-')
        start = int(start)
        end = int(end) if end else file_size - 1
    else:
        # If no range is specified, serve the whole file
        start, end = 0, file_size - 1

    # Command to transcode MKV to WebM using FFmpeg (streaming)
    command = [
        'ffmpeg',
        '-i', MKV_FILE_PATH,             # Input file (MKV)
        '-c:v', 'libvpx',                # Video codec (WebM - VP8)
        '-c:a', 'libvorbis',             # Audio codec (WebM - Vorbis)
        '-f', 'webm',                    # Output format (WebM)
        '-ss', str(start),               # Start from the byte range
        '-t', str(end - start + 1),      # Transcoding for the byte range
        '-',                             # Output to stdout (streaming)
    ]

    # Open the FFmpeg subprocess for streaming
    ffmpeg_process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE)

    # Generate the response by yielding larger chunks (e.g., 2x the current buffer size)
    def generate():
        chunk_size = 2048  # 2KB chunks, adjust this for performance
        while True:
            chunk = ffmpeg_process.stdout.read(chunk_size)
            if not chunk:
                break
            yield chunk

    # Return the response with the proper headers for byte range streaming
    response = Response(generate(), content_type='video/webm', status=206)
    response.headers['Content-Range'] = f'bytes {start}-{end}/{file_size}'
    response.headers['Accept-Ranges'] = 'bytes'
    response.headers['Content-Length'] = str(end - start + 1)

    # CORS headers
    response.headers['Access-Control-Allow-Origin'] = '*'
    response.headers['Access-Control-Allow-Methods'] = 'GET, OPTIONS'
    response.headers['Access-Control-Allow-Headers'] = 'Range'

    # Handle errors if FFmpeg fails
    ffmpeg_stderr = ffmpeg_process.stderr.read()
    if ffmpeg_stderr:
        print(f"FFmpeg error: {ffmpeg_stderr.decode('utf-8')}")
        response = Response("Error streaming video", status=500)

    return response

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=2255)
