from flask import Flask, Response, request
import os
import subprocess
from flask_cors import CORS

app = Flask(__name__)
CORS(app, resources={r"/*": {"origins": "*"}})  # Enable CORS for all routes

# Path to your MKV file
MKV_FILE_PATH = 'video.mkv'

def parse_progress(stderr_output):
    # Check if stderr_output is a bytes object or a string
    if isinstance(stderr_output, bytes):
        stderr_output = stderr_output.decode('utf-8')  # Decode bytes to string
    
    # Now `stderr_output` is definitely a string, so we can proceed
    for line in stderr_output.splitlines():
        # Process the line (look for progress information, etc.)
        if 'time=' in line:
            time_info = line.split('time=')[1].split(' ')[0]
            return f"Progress: {time_info}"
    return "No progress info found"

@app.route('/stream')
def stream_video():
    # Check if the video file exists
    if not os.path.exists(MKV_FILE_PATH):
        return "Video file not found", 404

    # Get the range (start time) from the request, default to 0 if not provided
    start_time = request.args.get('start', 0, type=int)

    # Set the end time as 60 seconds after the start time (1 minute)
    end_time = start_time + 60

    # Command to copy MKV streams to WebM (without transcoding)
    command = [
        'ffmpeg',
        '-i', MKV_FILE_PATH,             # Input MKV file
        '-ss', str(start_time),           # Start time for the segment
        '-to', str(end_time),             # End time for the segment
        '-c:v', 'copy',                   # Copy video stream (no re-encoding)
        '-c:a', 'copy',                   # Copy audio stream (no re-encoding)
        '-f', 'webm',                     # Set output format to WebM
        '-movflags', 'frag_keyframe+empty_moov',  # Flags to make WebM better suited for progressive playback
        '-'                              # Output to stdout for streaming
    ]

    # Open the FFmpeg process for copying the streams
    ffmpeg_process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE)

    # Read stderr for progress (to send feedback to the client)
    stderr_output = ffmpeg_process.stderr.read()
    progress = parse_progress(stderr_output)

    # Log the current progress
    print(progress)  # You can also send this to the client if needed

    # Generate the response by streaming the video from FFmpeg's stdout
    def generate():
        while True:
            chunk = ffmpeg_process.stdout.read(1024*1024)  # Read 1MB chunks
            if not chunk:
                break
            yield chunk

    # Return the response with appropriate headers
    response = Response(generate(), content_type='video/webm', status=200)
    response.headers['Access-Control-Allow-Origin'] = '*'
    response.headers['Access-Control-Allow-Methods'] = 'GET, OPTIONS'
    response.headers['Access-Control-Allow-Headers'] = 'Range'

    return response

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=2255)
