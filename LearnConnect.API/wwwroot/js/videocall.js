/**
 * LearnConnect Video Call Module
 * Uses WebRTC (peer-to-peer) + SignalR for signaling.
 * Supports: audio/video toggle, screen sharing, call timer, chat during call.
 */
class VideoCallManager {
  constructor() {
    this.connection = null;       // SignalR connection
    this.peerConnection = null;   // RTCPeerConnection
    this.localStream = null;      // Local media stream
    this.roomId = null;
    this.myConnectionId = null;
    this.remoteConnectionId = null;
    this.isMuted = false;
    this.isVideoOff = false;
    this.isScreenSharing = false;
    this.callStartTime = null;
    this.timerInterval = null;
    this.screenStream = null;

    // Stop media on tab close/nav
    window.addEventListener('beforeunload', () => this.cleanup());
    window.addEventListener('pagehide', () => this.cleanup());

    // STUN/TURN servers - free Google STUN
    this.iceServers = {
      iceServers: [
        { urls: 'stun:stun.l.google.com:19302' },
        { urls: 'stun:stun1.l.google.com:19302' },
        { urls: 'stun:stun2.l.google.com:19302' }
      ]
    };
  }

  // ─────────────────────────────────────────────────────────────────
  // ENTRY POINT: Join a video room by reservationId
  // ─────────────────────────────────────────────────────────────────
  async joinRoom(reservationId) {
    try {
      // 1. Get room info from backend
      const roomInfo = await api.getVideoCallRoom(reservationId);
      this.roomId = roomInfo.roomId;
      this.roomInfo = roomInfo;

      // 2. Build call UI
      this.renderCallUI(roomInfo);

      // 3. Get local media
      await this.setupLocalMedia();

      // 4. Connect to SignalR hub
      await this.connectSignalR();

      // 5. Join room
      await this.connection.invoke('JoinRoom', this.roomId, String(AppState.user?.userId || ''));

    } catch (err) {
      console.error('Failed to join room:', err);
      this.showError(err.message || 'Could not start video call');
    }
  }

  // ─────────────────────────────────────────────────────────────────
  // RENDER UI
  // ─────────────────────────────────────────────────────────────────
  renderCallUI(roomInfo) {
    const container = document.getElementById('app');
    container.innerHTML = `
      <div class="call-page" id="callPage">
        <!-- Videos -->
        <div class="call-videos">
          <div class="video-wrapper remote-wrapper" id="remoteWrapper">
            <video id="remoteVideo" autoplay playsinline></video>
            <div class="remote-label" id="remoteLabel">
              <span class="call-dot"></span>
              <span id="remoteName">Waiting for other participant...</span>
            </div>
          </div>
          <div class="video-wrapper local-wrapper" id="localWrapper">
            <video id="localVideo" autoplay playsinline muted></video>
            <div class="local-label">You</div>
          </div>
        </div>

        <!-- Status bar -->
        <div class="call-status-bar">
          <div class="call-info">
            <div class="call-participants">
              <span class="participant-badge">🎓 ${roomInfo.teacherName}</span>
              <span style="color:var(--gray-400)">↔</span>
              <span class="participant-badge">📚 ${roomInfo.studentName}</span>
            </div>
            <div class="call-timer" id="callTimer">00:00</div>
          </div>
          <div class="call-quality" id="callQuality">
            <span class="quality-dot good"></span> Connecting...
          </div>
        </div>

        <!-- Controls -->
        <div class="call-controls">
          <button class="ctrl-btn" id="btnMic" onclick="videoCallManager.toggleMute()" title="Toggle Microphone">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 14c1.66 0 3-1.34 3-3V5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3zm-1-9h2v6h-2V5z"/><path d="M17 11c0 2.76-2.24 5-5 5s-5-2.24-5-5H5c0 3.53 2.61 6.43 6 6.92V21h2v-3.08c3.39-.49 6-3.39 6-6.92h-2z"/></svg>
            <span>Mute</span>
          </button>

          <button class="ctrl-btn" id="btnVideo" onclick="videoCallManager.toggleVideo()" title="Toggle Camera">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M17 10.5V7c0-.55-.45-1-1-1H4c-.55 0-1 .45-1 1v10c0 .55.45 1 1 1h12c.55 0 1-.45 1-1v-3.5l4 4v-11l-4 4z"/></svg>
            <span>Camera</span>
          </button>

          <button class="ctrl-btn" id="btnScreen" onclick="videoCallManager.toggleScreenShare()" title="Share Screen">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M20 3H4c-1.1 0-2 .9-2 2v11c0 1.1.9 2 2 2h3l-1 1v1h12v-1l-1-1h3c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 13H4V5h16v11z"/></svg>
            <span>Share</span>
          </button>

          <button class="ctrl-btn end-call-btn" onclick="videoCallManager.endCall()" title="End Call">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M6.62 10.79c1.44 2.83 3.76 5.14 6.59 6.59l2.2-2.2c.27-.27.67-.36 1.02-.24 1.12.37 2.33.57 3.57.57.55 0 1 .45 1 1V20c0 .55-.45 1-1 1-9.39 0-17-7.61-17-17 0-.55.45-1 1-1h3.5c.55 0 1 .45 1 1 0 1.25.2 2.45.57 3.57.11.35.03.74-.25 1.02l-2.2 2.2z"/></svg>
            <span>End</span>
          </button>
        </div>

        <!-- Chat panel -->
        <div class="call-chat" id="callChat">
          <div class="chat-header">
            <span>💬 Call Chat</span>
            <button onclick="videoCallManager.toggleChat()">✕</button>
          </div>
          <div class="chat-messages" id="chatMessages"></div>
          <div class="chat-input-row">
            <input type="text" id="chatInput" class="form-input" placeholder="Type a message..." onkeydown="if(event.key==='Enter') videoCallManager.sendChatMessage()">
            <button class="btn btn-primary btn-sm" onclick="videoCallManager.sendChatMessage()">Send</button>
          </div>
        </div>

        <button class="chat-toggle-btn" id="chatToggleBtn" onclick="videoCallManager.toggleChat()" title="Chat">
          💬 <span id="chatBadge" style="display:none" class="badge">0</span>
        </button>
      </div>
    `;

    this.injectCallStyles();
  }

  // ─────────────────────────────────────────────────────────────────
  // MEDIA
  // ─────────────────────────────────────────────────────────────────
  async setupLocalMedia() {
    try {
      this.localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
      document.getElementById('localVideo').srcObject = this.localStream;
    } catch (err) {
      // Try audio only
      try {
        this.localStream = await navigator.mediaDevices.getUserMedia({ video: false, audio: true });
        document.getElementById('localVideo').srcObject = this.localStream;
        this.isVideoOff = true;
        this.updateVideoButton();
      } catch (audioErr) {
        throw new Error('Camera and microphone access denied. Please allow permissions.');
      }
    }
  }

  // ─────────────────────────────────────────────────────────────────
  // SIGNALR
  // ─────────────────────────────────────────────────────────────────
  async connectSignalR() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/videocall', {
        accessTokenFactory: () => AppState.token
      })
      .withAutomaticReconnect()
      .build();

    // Events from hub
    this.connection.on('RoomMembers', async (existingMembers) => {
      // There are already people in the room — initiate offer
      if (existingMembers.length > 0) {
        this.remoteConnectionId = existingMembers[0];
        await this.createPeerConnection();
        await this.createOffer(this.remoteConnectionId);
      }
    });

    this.connection.on('PeerJoined', async (connId, userId) => {
      this.remoteConnectionId = connId;
      // Responder: wait for offer (don't create offer here)
      await this.createPeerConnection();
      this.updateQuality('Peer joined, connecting...');
    });

    this.connection.on('ReceiveOffer', async (fromConnId, sdpOffer) => {
      this.remoteConnectionId = fromConnId;
      if (!this.peerConnection) await this.createPeerConnection();
      await this.peerConnection.setRemoteDescription(new RTCSessionDescription({ type: 'offer', sdp: sdpOffer }));
      const answer = await this.peerConnection.createAnswer();
      await this.peerConnection.setLocalDescription(answer);
      await this.connection.invoke('SendAnswer', fromConnId, answer.sdp);
    });

    this.connection.on('ReceiveAnswer', async (fromConnId, sdpAnswer) => {
      await this.peerConnection.setRemoteDescription(new RTCSessionDescription({ type: 'answer', sdp: sdpAnswer }));
    });

    this.connection.on('ReceiveIceCandidate', async (fromConnId, candidateJson) => {
      try {
        const candidate = JSON.parse(candidateJson);
        await this.peerConnection.addIceCandidate(new RTCIceCandidate(candidate));
      } catch (e) { /* ignore */ }
    });

    this.connection.on('PeerMutedChanged', (connId, isMuted) => {
      const label = document.getElementById('remoteName');
      if (label) label.textContent = (this.roomInfo?.teacherName || 'Participant') + (isMuted ? ' 🔇' : '');
    });

    this.connection.on('PeerVideoChanged', (connId, isOff) => {
      const remoteVideo = document.getElementById('remoteVideo');
      if (remoteVideo) remoteVideo.style.display = isOff ? 'none' : 'block';
    });

    this.connection.on('CallEnded', () => {
      this.showCallEnded('The other participant ended the call.');
    });

    this.connection.on('PeerLeft', () => {
      this.updateQuality('Participant left the call');
      this.remoteConnectionId = null;
    });

    await this.connection.start();
    this.myConnectionId = this.connection.connectionId;
  }

  // ─────────────────────────────────────────────────────────────────
  // WEBRTC
  // ─────────────────────────────────────────────────────────────────
  async createPeerConnection() {
    this.peerConnection = new RTCPeerConnection(this.iceServers);

    // Add local tracks
    this.localStream.getTracks().forEach(track => {
      this.peerConnection.addTrack(track, this.localStream);
    });

    // Receive remote tracks
    this.peerConnection.ontrack = (event) => {
      const remoteVideo = document.getElementById('remoteVideo');
      if (remoteVideo && event.streams[0]) {
        remoteVideo.srcObject = event.streams[0];
        document.getElementById('remoteName').textContent = this.roomInfo?.teacherName || 'Participant';
        this.startTimer();
        this.updateQuality('Connected ✓');
      }
    };

    // ICE candidates
    this.peerConnection.onicecandidate = async (event) => {
      if (event.candidate && this.remoteConnectionId) {
        await this.connection.invoke(
          'SendIceCandidate',
          this.remoteConnectionId,
          JSON.stringify(event.candidate)
        );
      }
    };

    this.peerConnection.onconnectionstatechange = () => {
      const state = this.peerConnection.connectionState;
      const map = {
        'connected': 'Connected ✓',
        'connecting': 'Connecting...',
        'disconnected': 'Disconnected',
        'failed': 'Connection failed',
        'closed': 'Call ended'
      };
      this.updateQuality(map[state] || state);
    };
  }

  async createOffer(targetConnId) {
    const offer = await this.peerConnection.createOffer();
    await this.peerConnection.setLocalDescription(offer);
    await this.connection.invoke('SendOffer', targetConnId, offer.sdp);
  }

  // ─────────────────────────────────────────────────────────────────
  // CONTROLS
  // ─────────────────────────────────────────────────────────────────
  toggleMute() {
    this.isMuted = !this.isMuted;
    this.localStream.getAudioTracks().forEach(t => t.enabled = !this.isMuted);
    this.updateMuteButton();
    this.connection?.invoke('ToggleMute', this.roomId, this.isMuted).catch(() => { });
  }

  updateMuteButton() {
    const btn = document.getElementById('btnMic');
    if (!btn) return;
    if (this.isMuted) {
      btn.classList.add('ctrl-off');
      btn.querySelector('span').textContent = 'Unmute';
    } else {
      btn.classList.remove('ctrl-off');
      btn.querySelector('span').textContent = 'Mute';
    }
  }

  toggleVideo() {
    this.isVideoOff = !this.isVideoOff;
    this.localStream.getVideoTracks().forEach(t => t.enabled = !this.isVideoOff);
    this.updateVideoButton();
    this.connection?.invoke('ToggleVideo', this.roomId, this.isVideoOff).catch(() => { });
  }

  updateVideoButton() {
    const btn = document.getElementById('btnVideo');
    if (!btn) return;
    if (this.isVideoOff) {
      btn.classList.add('ctrl-off');
      btn.querySelector('span').textContent = 'Show';
    } else {
      btn.classList.remove('ctrl-off');
      btn.querySelector('span').textContent = 'Camera';
    }
  }

  async toggleScreenShare() {
    if (!this.isScreenSharing) {
      try {
        const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
        const screenTrack = screenStream.getVideoTracks()[0];

        // Replace video track in peer connection
        const sender = this.peerConnection?.getSenders().find(s => s.track?.kind === 'video');
        if (sender) await sender.replaceTrack(screenTrack);

        // Show screen in local video
        document.getElementById('localVideo').srcObject = screenStream;
        this.screenStream = screenStream;

        screenTrack.onended = () => this.stopScreenShare();
        this.isScreenSharing = true;
        document.getElementById('btnScreen').classList.add('active');
        document.getElementById('btnScreen').querySelector('span').textContent = 'Stop Share';
      } catch (err) { /* user cancelled */ }
    } else {
      this.stopScreenShare();
    }
  }

  async stopScreenShare() {
    this.isScreenSharing = false;
    const videoTrack = this.localStream.getVideoTracks()[0];
    const sender = this.peerConnection?.getSenders().find(s => s.track?.kind === 'video');
    if (sender && videoTrack) await sender.replaceTrack(videoTrack);
    document.getElementById('localVideo').srcObject = this.localStream;
    const btn = document.getElementById('btnScreen');
    if (btn) { btn.classList.remove('active'); btn.querySelector('span').textContent = 'Share'; }
  }

  async endCall() {
    if (confirm('Are you sure you want to end the call?')) {
      await this.connection?.invoke('EndCall', this.roomId).catch(() => { });
      this.cleanup();
      window.app?.navigate('dashboard');
    }
  }

  // ─────────────────────────────────────────────────────────────────
  // CHAT
  // ─────────────────────────────────────────────────────────────────
  toggleChat() {
    const chat = document.getElementById('callChat');
    const badge = document.getElementById('chatBadge');
    if (chat) {
      chat.classList.toggle('open');
      if (chat.classList.contains('open')) {
        badge.style.display = 'none';
        badge.textContent = '0';
      }
    }
  }

  sendChatMessage() {
    const input = document.getElementById('chatInput');
    const msg = input?.value.trim();
    if (!msg) return;
    this.appendChatMessage('You', msg, true);
    // In a real app, send via SignalR. For demo, local only.
    input.value = '';
  }

  appendChatMessage(sender, text, isMine) {
    const msgs = document.getElementById('chatMessages');
    if (!msgs) return;
    const div = document.createElement('div');
    div.className = `chat-msg ${isMine ? 'mine' : 'theirs'}`;
    div.innerHTML = `<div class="chat-sender">${sender}</div><div class="chat-bubble">${text}</div>`;
    msgs.appendChild(div);
    msgs.scrollTop = msgs.scrollHeight;

    if (!isMine) {
      const badge = document.getElementById('chatBadge');
      const chat = document.getElementById('callChat');
      if (badge && !chat?.classList.contains('open')) {
        badge.style.display = 'inline';
        badge.textContent = parseInt(badge.textContent || '0') + 1;
      }
    }
  }

  // ─────────────────────────────────────────────────────────────────
  // TIMER & HELPERS
  // ─────────────────────────────────────────────────────────────────
  startTimer() {
    this.callStartTime = Date.now();
    this.timerInterval = setInterval(() => {
      const elapsed = Math.floor((Date.now() - this.callStartTime) / 1000);
      const m = String(Math.floor(elapsed / 60)).padStart(2, '0');
      const s = String(elapsed % 60).padStart(2, '0');
      const el = document.getElementById('callTimer');
      if (el) el.textContent = `${m}:${s}`;
    }, 1000);
  }

  updateQuality(text) {
    const el = document.getElementById('callQuality');
    if (el) el.innerHTML = `<span class="quality-dot good"></span> ${text}`;
  }

  showCallEnded(reason) {
    const page = document.getElementById('callPage');
    if (page) {
      page.innerHTML = `
        <div style="display:flex;flex-direction:column;align-items:center;justify-content:center;height:100vh;gap:1.5rem;background:#0f172a;color:white;">
          <div style="font-size:4rem;">📞</div>
          <h2 style="font-size:1.5rem;">${reason}</h2>
          <p style="color:#94a3b8;">Call duration: ${document.getElementById('callTimer')?.textContent || '00:00'}</p>
          <button class="btn btn-primary" onclick="window.app.navigate('dashboard')">Back to Dashboard</button>
        </div>
      `;
    }
    this.cleanup();
  }

  showError(msg) {
    const app = document.getElementById('app');
    if (app) app.innerHTML = `
      <div style="display:flex;flex-direction:column;align-items:center;justify-content:center;height:100vh;gap:1rem;background:#0f172a;color:white;">
        <div style="font-size:3rem;">❌</div>
        <h2>Could not start call</h2><p style="color:#f87171;">${msg}</p>
        <button class="btn btn-secondary" onclick="window.app.navigate('dashboard')">Back to Dashboard</button>
      </div>`;
  }

  cleanup() {
    console.log('Cleaning up video call...');
    if (this.timerInterval) clearInterval(this.timerInterval);

    // Stop ALL tracks (camera/mic/screen)
    if (this.localStream) {
      this.localStream.getTracks().forEach(t => {
        t.stop();
        t.enabled = false;
      });
    }
    if (this.screenStream) {
      this.screenStream.getTracks().forEach(t => {
        t.stop();
        t.enabled = false;
      });
    }

    this.peerConnection?.close();
    this.connection?.stop();
    this.peerConnection = null;
    this.localStream = null;
    this.screenStream = null;
    this.connection = null;
    this.roomId = null;
    this.isScreenSharing = false;
  }

  // ─────────────────────────────────────────────────────────────────
  // STYLES (injected dynamically)
  // ─────────────────────────────────────────────────────────────────
  injectCallStyles() {
    if (document.getElementById('callStyles')) return;
    const style = document.createElement('style');
    style.id = 'callStyles';
    style.textContent = `
      .call-page {
        position: fixed; inset: 0; background: #0f172a; display: flex;
        flex-direction: column; font-family: 'Inter', sans-serif; z-index: 9999;
      }
      .call-videos {
        flex: 1; position: relative; background: #020617;
        display: flex; align-items: center; justify-content: center;
      }
      .remote-wrapper {
        width: 100%; height: 100%; background: #1e293b;
        display: flex; align-items: center; justify-content: center;
      }
      #remoteVideo { width: 100%; height: 100%; object-fit: cover; }
      .local-wrapper {
        position: absolute; bottom: 1rem; right: 1rem;
        width: 180px; height: 120px; border-radius: 12px;
        overflow: hidden; border: 2px solid #334155;
        background: #1e293b; box-shadow: 0 8px 32px rgba(0,0,0,.5);
      }
      #localVideo { width: 100%; height: 100%; object-fit: cover; transform: scaleX(-1); }
      .local-label {
        position: absolute; bottom: 6px; left: 8px;
        font-size: 0.7rem; color: white; background: rgba(0,0,0,.5);
        padding: 2px 6px; border-radius: 4px;
      }
      .remote-label {
        position: absolute; top: 1rem; left: 1rem;
        display: flex; align-items: center; gap: 0.5rem;
        font-size: 0.85rem; color: white; background: rgba(0,0,0,.5);
        padding: 4px 12px; border-radius: 20px;
      }
      .call-dot { width: 8px; height: 8px; border-radius: 50%;
        background: #22c55e; display: inline-block; animation: pulse 2s infinite; }
      @keyframes pulse { 0%,100%{opacity:1} 50%{opacity:.4} }
      .call-status-bar {
        display: flex; justify-content: space-between; align-items: center;
        padding: 0.5rem 1.5rem; background: #0f172a; border-top: 1px solid #1e293b;
      }
      .call-info { display: flex; align-items: center; gap: 1rem; }
      .participant-badge {
        background: #1e293b; color: #cbd5e1; padding: 4px 10px;
        border-radius: 20px; font-size: 0.8rem;
      }
      .call-timer { color: #22c55e; font-weight: 700; font-size: 1.1rem; font-family: monospace; }
      .call-quality { font-size: 0.8rem; color: #94a3b8; display: flex; align-items: center; gap: 6px; }
      .quality-dot { width: 8px; height: 8px; border-radius: 50%; background: #22c55e; display: inline-block; }
      .call-controls {
        display: flex; justify-content: center; gap: 1rem;
        padding: 1rem 1.5rem; background: #0f172a;
      }
      .ctrl-btn {
        display: flex; flex-direction: column; align-items: center; gap: 4px;
        background: #1e293b; border: none; border-radius: 12px;
        color: white; cursor: pointer; padding: 0.75rem 1.25rem;
        transition: all .2s; font-size: 0.75rem; min-width: 70px;
      }
      .ctrl-btn svg { width: 24px; height: 24px; }
      .ctrl-btn:hover { background: #334155; transform: scale(1.05); }
      .ctrl-btn.ctrl-off { background: #374151; color: #f87171; }
      .ctrl-btn.active { background: #0ea5e9; }
      .end-call-btn { background: #dc2626 !important; min-width: 90px; }
      .end-call-btn:hover { background: #b91c1c !important; }
      .call-chat {
        position: fixed; right: 1rem; bottom: 5rem; width: 300px;
        background: #1e293b; border-radius: 12px; border: 1px solid #334155;
        display: none; flex-direction: column;
        box-shadow: 0 8px 32px rgba(0,0,0,.5); z-index: 10000;
      }
      .call-chat.open { display: flex; }
      .chat-header {
        display: flex; justify-content: space-between; align-items: center;
        padding: 0.75rem 1rem; border-bottom: 1px solid #334155;
        color: white; font-weight: 600;
      }
      .chat-header button { background: none; border: none; color: #94a3b8; cursor: pointer; font-size: 1rem; }
      .chat-messages { flex: 1; max-height: 250px; overflow-y: auto; padding: 0.75rem; display: flex; flex-direction: column; gap: 0.5rem; }
      .chat-msg { display: flex; flex-direction: column; gap: 2px; }
      .chat-msg.mine { align-items: flex-end; }
      .chat-sender { font-size: 0.7rem; color: #94a3b8; }
      .chat-bubble {
        background: #334155; color: white; padding: 6px 12px;
        border-radius: 12px; font-size: 0.85rem; max-width: 80%;
      }
      .chat-msg.mine .chat-bubble { background: #0ea5e9; }
      .chat-input-row { display: flex; gap: 0.5rem; padding: 0.75rem; border-top: 1px solid #334155; }
      .chat-input-row .form-input { flex: 1; background: #0f172a; border-color: #334155; color: white; font-size: 0.85rem; padding: 0.4rem 0.75rem; }
      .chat-toggle-btn {
        position: fixed; right: 1rem; bottom: 4.5rem;
        background: #0ea5e9; border: none; border-radius: 50%;
        width: 48px; height: 48px; color: white; font-size: 1.2rem;
        cursor: pointer; z-index: 10001;
        box-shadow: 0 4px 12px rgba(14,165,233,.5);
        display: flex; align-items: center; justify-content: center;
      }
      .badge {
        background: #dc2626; color: white; border-radius: 50%;
        width: 18px; height: 18px; font-size: 0.65rem;
        display: inline-flex; align-items: center; justify-content: center;
        position: absolute; top: 0; right: 0;
      }
    `;
    document.head.appendChild(style);
  }
}

const videoCallManager = new VideoCallManager();
