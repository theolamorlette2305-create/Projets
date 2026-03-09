import os
from flask import Flask, render_template, request
import yt_dlp
import whisper
import subprocess
import torch

app = Flask(__name__)

OUTPUT_FOLDER = 'static'
os.makedirs(OUTPUT_FOLDER, exist_ok=True)

@app.route('/')
def home():
    
    return render_template('index.html')

@app.route('/process', methods=['POST'])
def process_video():
    video_url = request.form['video_url']
    
    want_subtitles = 'option_subtitles' in request.form
    want_audio_boost = 'option_audio_boost' in request.form

    input_video = os.path.join(OUTPUT_FOLDER, "input.mp4")
    subtitle_file = os.path.join(OUTPUT_FOLDER, "subtitles.srt")
    output_video = os.path.join(OUTPUT_FOLDER, "output_accessible.mp4")

    print("1. Téléchargement...")
    try:
        current_folder = os.path.dirname(os.path.abspath(__file__))
        ydl_opts = {
            'format': 'bestvideo+bestaudio/best', 
            'outtmpl': input_video,
            'overwrites': True,
            'merge_output_format': 'mp4',       
            'ffmpeg_location': current_folder,  
            'quiet': True,
            'no_warnings': True
        }
        with yt_dlp.YoutubeDL(ydl_opts) as ydl:
            ydl.download([video_url])
            
    except Exception as e:
        print(f"ERREUR TÉLÉCHARGEMENT : {e}")
        return render_template('result.html', error=f"Erreur de téléchargement : {e}")

    cmd = ['ffmpeg', '-y', '-i', input_video]
    video_filters = [] 
    audio_filters = [] 

    if want_subtitles:
        print("2. Génération des sous-titres (IA)...")
        device = "cuda" if torch.cuda.is_available() else "cpu"
        model = whisper.load_model("small", device=device)
        result = model.transcribe(input_video, fp16=False, language="fr", beam_size=5)

        with open(subtitle_file, "w", encoding="utf-8") as f:
            for i, segment in enumerate(result["segments"]):
                start = format_timestamp(segment['start'])
                end = format_timestamp(segment['end'])
                text = segment['text'].strip()
                f.write(f"{i + 1}\n{start} --> {end}\n{text}\n\n")
        
        path_unix = subtitle_file.replace('\\', '/')
        style = "Fontsize=24,PrimaryColour=&H00FFFF,BackColour=&H80000000,BorderStyle=3"
        video_filters.append(f"subtitles={path_unix}:force_style='{style}'")

    if want_audio_boost:
        print("3. Application du Boost Audio...")
        audio_filters.append("highpass=f=200, equalizer=f=3000:width_type=h:width=1000:g=15, dynaudnorm")

    if video_filters:
        cmd.append('-vf')
        cmd.append(",".join(video_filters)) 
    else:
        cmd.extend(['-c:v', 'copy'])

    if audio_filters:
        cmd.append('-af')
        cmd.append(",".join(audio_filters))
    else:
        cmd.extend(['-c:a', 'copy']) 

    cmd.append(output_video)

    print("Exécution FFmpeg...")
    subprocess.run(cmd)

    options_summary = []
    if want_subtitles: options_summary.append("Sous-titres contrastés")
    if want_audio_boost: options_summary.append("Clarté vocale")
    if not options_summary: options_summary.append("Aucune modification (Copie originale)")

    return render_template('result.html', video_file="output_accessible.mp4", options=", ".join(options_summary))

def format_timestamp(seconds):
    hours = int(seconds // 3600)
    minutes = int((seconds % 3600) // 60)
    secs = int(seconds % 60)
    millis = int((seconds - int(seconds)) * 1000)
    return f"{hours:02}:{minutes:02}:{secs:02},{millis:03}"

if __name__ == '__main__':
    app.run(debug=True)