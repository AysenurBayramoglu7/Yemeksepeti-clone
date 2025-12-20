// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function favoriEkle(restoranId, event, btnElement) {
    // Kartın linkine gitmesini engelle
    event.preventDefault();
    event.stopPropagation();

    // İkonu seçelim
    // Not: btnElement bir <button> olduğu için içindeki <i>'yi bulmalıyız
    const icon = btnElement.querySelector('i');
    
    // Eğer icon seçilemediyse (belki doğrudan i'ye tıklandıysa) btnElement'in kendisine bak
    const targetIcon = icon || (btnElement.tagName === 'I' ? btnElement : btnElement.querySelector('.bi'));

    // İkon dolu kalp mi?
    const isFavorite = targetIcon.classList.contains('bi-heart-fill');
    const url = isFavorite ? '/Favori/Sil' : '/Favori/Ekle';

    fetch(url + '?restoranId=' + restoranId, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            if (isFavorite) {
                targetIcon.classList.remove('bi-heart-fill', 'text-danger');
                targetIcon.classList.add('bi-heart', 'text-secondary');
            } else {
                targetIcon.classList.remove('bi-heart', 'text-secondary');
                targetIcon.classList.add('bi-heart-fill', 'text-danger');
            }
        } else {
            if (data.redirectUrl) {
                window.location.href = data.redirectUrl;
            } else {
                alert(data.message || "Bir hata oluştu.");
            }
        }
    })
    .catch(error => {
        console.error('Hata:', error);
        alert('Bir hata oluştu.');
    });
}
