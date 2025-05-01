const stars = document.querySelectorAll('#star-container .star');
const ratingInput = document.getElementById('ratingInput');

stars.forEach(star => {
    star.addEventListener('click', () => {
        const value = parseInt(star.getAttribute('data-value'));

        // Guarda el valor
        ratingInput.value = value;

        // Limpia todas
        stars.forEach(s => s.classList.remove('selected'));

        // Marca las correctas
        stars.forEach(s => {
            if (parseInt(s.getAttribute('data-value')) <= value) {
                s.classList.add('selected');
            }
        });
    });
});
