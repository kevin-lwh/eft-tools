import pyaudio
from google.cloud import speech

# Audio recording parameters
RATE = 16000
CHUNK = int(RATE / 10)  # 100ms
RECORD_SECONDS = 2.5

def record_audio():
    audio = pyaudio.PyAudio()
    stream = audio.open(format=pyaudio.paInt16, channels=1, rate=RATE, input=True, frames_per_buffer=CHUNK)
    
    Recordframes = []
    for i in range(0, int(RATE / CHUNK * RECORD_SECONDS)):
        data = stream.read(CHUNK)
        Recordframes.append(data)
    
    stream.stop_stream()
    stream.close()
    audio.terminate()

    return b''.join(Recordframes)


def transcribe_audio(data):
    """Transcribe the given audio file."""
    client = speech.SpeechClient()
    audio = speech.RecognitionAudio(content=data)
    config = speech.RecognitionConfig(
        encoding=speech.RecognitionConfig.AudioEncoding.LINEAR16,
        sample_rate_hertz=16000,
        language_code="en-US",
    )

    response = client.recognize(config=config, audio=audio)

    text = ""
    # Each result is for a consecutive portion of the audio. Iterate through
    # them to get the transcripts for the entire audio file.
    for result in response.results:
        # The first alternative is the most likely one for this portion.
        text = u"{}".format(result.alternatives[0].transcript)
    
    return text

data = record_audio()
text = transcribe_audio(data)
print(text)