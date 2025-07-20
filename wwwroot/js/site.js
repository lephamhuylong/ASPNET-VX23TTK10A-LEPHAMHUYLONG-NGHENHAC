document.addEventListener('DOMContentLoaded', function () {

  
    function initializePlayer() {
        const audioPlayer = document.getElementById('audio-player');
        const playerContainer = document.getElementById('player-container');
        const playerImage = document.getElementById('player-image');
        const playerTitle = document.getElementById('player-title');
        const playerArtist = document.getElementById('player-artist');
        const playPauseBtn = document.getElementById('btn-play-pause');
        const progressBar = document.getElementById('progress-bar');

        if (!audioPlayer || !playerContainer || !playerImage || !playerTitle || !playerArtist || !playPauseBtn || !progressBar) {
            console.error("Một hoặc nhiều thành phần của trình phát nhạc không được tìm thấy. Player sẽ không hoạt động.");
            return;
        }

        document.querySelectorAll('.song-card').forEach(card => {
            card.addEventListener('click', function () {
                const songData = {
                    id: this.dataset.songId,
                    title: this.dataset.title,
                    artist: this.dataset.artist,
                    imageUrl: this.dataset.imageUrl,
                    filePath: this.dataset.filePath
                };
                playSong(songData);
            });
        });

        async function playSong(songData) {
            playerImage.src = songData.imageUrl || 'https://placehold.co/60x60/212529/FFF?text=Music';
            playerTitle.textContent = songData.title;
            playerArtist.textContent = songData.artist;
            audioPlayer.src = songData.filePath;
            audioPlayer.play();
            playerContainer.classList.add('active');
            updatePlayPauseIcon(true);

           
            try {
                await fetch(`/api/SongsApi/IncrementListenCount/${songData.id}`, {
                    method: 'POST'
                });
                console.log(`Đã ghi nhận lượt nghe cho bài hát ID: ${songData.id}`);
            } catch (error) {
                console.error("Lỗi khi ghi nhận lượt nghe:", error);
            }
        }

        playPauseBtn.addEventListener('click', function () {
            if (audioPlayer.paused) {
                audioPlayer.play();
            } else {
                audioPlayer.pause();
            }
        });

        audioPlayer.addEventListener('play', () => updatePlayPauseIcon(true));
        audioPlayer.addEventListener('pause', () => updatePlayPauseIcon(false));

        function updatePlayPauseIcon(isPlaying) {
            const icon = playPauseBtn.querySelector('i');
            if (icon) {
                icon.className = isPlaying ? 'fas fa-pause-circle' : 'fas fa-play-circle';
            }
        }

        audioPlayer.addEventListener('timeupdate', function () {
            const progress = (audioPlayer.currentTime / audioPlayer.duration) * 100;
            progressBar.value = isNaN(progress) ? 0 : progress;
        });

        progressBar.addEventListener('input', function () {
            const time = (progressBar.value / 100) * audioPlayer.duration;
            audioPlayer.currentTime = time;
        });
    }

 
    function initializePlaylist() {
        const addToPlaylistModalElement = document.getElementById('addToPlaylistModal');
        if (!addToPlaylistModalElement) return;

        const addToPlaylistModal = new bootstrap.Modal(addToPlaylistModalElement);
        const playlistSelectionDiv = document.getElementById('playlist-selection');
        const songNameModalSpan = document.getElementById('song-name-modal');
        let currentSongIdToAdd = null;

     
        document.querySelectorAll('.btn-add-to-playlist').forEach(button => {
            button.addEventListener('click', async function (event) {
                event.stopPropagation();

                currentSongIdToAdd = this.dataset.songId;
                songNameModalSpan.textContent = `"${this.dataset.songName}"`;

                try {
                    const response = await fetch('/Playlists/GetUserPlaylists');
                    if (!response.ok) {
                        alert("Vui lòng đăng nhập để sử dụng chức năng này.");
                        window.location.href = '/Identity/Account/Login';
                        return;
                    }
                    const playlists = await response.json();

                    playlistSelectionDiv.innerHTML = '';
                    if (playlists.length > 0) {
                        playlists.forEach(playlist => {
                            const playlistItem = document.createElement('a');
                            playlistItem.href = '#';
                            playlistItem.className = 'list-group-item list-group-item-action';
                            playlistItem.textContent = playlist.name;
                            playlistItem.dataset.playlistId = playlist.id;
                            playlistSelectionDiv.appendChild(playlistItem);
                        });
                    } else {
                        playlistSelectionDiv.innerHTML = '<p>Bạn chưa có playlist nào.</p>';
                    }
                    addToPlaylistModal.show();
                } catch (error) {
                    console.error('Lỗi khi lấy danh sách playlist:', error);
                    alert("Có lỗi xảy ra. Vui lòng đăng nhập và thử lại.");
                }
            });
        });


        playlistSelectionDiv.addEventListener('click', async function (event) {
            event.preventDefault();
            if (event.target.matches('.list-group-item-action')) {
                const playlistId = event.target.dataset.playlistId;

                try {
                    const response = await fetch('/Playlists/AddSongToPlaylist', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        body: `songId=${currentSongIdToAdd}&playlistId=${playlistId}`
                    });
                    const result = await response.json();
                    alert(result.message);
                    addToPlaylistModal.hide();
                } catch (error) {
                    console.error('Lỗi khi thêm bài hát vào playlist:', error);
                    alert("Có lỗi xảy ra, không thể thêm bài hát.");
                }
            }
        });

    
        document.querySelectorAll('.btn-remove-from-playlist').forEach(button => {
            button.addEventListener('click', async function (event) {
                event.stopPropagation();

                const songId = this.dataset.songId;
                const playlistId = this.dataset.playlistId;

                if (confirm("Bạn có chắc chắn muốn xóa bài hát này khỏi playlist?")) {
                    try {
                        const response = await fetch('/Playlists/RemoveSongFromPlaylist', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            body: `songId=${songId}&playlistId=${playlistId}`
                        });

                        const result = await response.json();
                        alert(result.message);

                        if (result.success) {
                        
                            document.getElementById(`song-${songId}`).remove();
                        }
                    } catch (error) {
                        console.error("Lỗi khi xóa bài hát:", error);
                        alert("Đã có lỗi xảy ra.");
                    }
                }
            });
        });
    }

 
    initializePlayer();
    initializePlaylist();
});
