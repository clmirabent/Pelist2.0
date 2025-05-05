document.addEventListener("DOMContentLoaded", function () {
    // ⭐ FILTRAR REVIEWS POR ESTRELLAS
    document.querySelectorAll('.filter-btn').forEach(button => {
        button.addEventListener('click', () => {
            const selectedRating = button.getAttribute('data-rating');
            const reviews = document.querySelectorAll('.review-item');
            let visibleCount = 0;

            reviews.forEach(review => {
                const rating = review.getAttribute('data-rating');

                if (selectedRating === 'all' || parseInt(rating) === parseInt(selectedRating)) {
                    review.classList.remove('hidden');
                    visibleCount++;
                } else {
                    review.classList.add('hidden');
                }
            });

            // Mostrar u ocultar mensaje si no hay coincidencias
            const noReviewsMessage = document.getElementById('noReviewsMessage');
            if (noReviewsMessage) {
                noReviewsMessage.style.display = visibleCount === 0 ? 'block' : 'none';
            }

            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            button.classList.add('active');
        });
    });

    // 🔁 CARGAR MÁS REVIEWS
    document.getElementById("loadMoreBtn")?.addEventListener("click", function () {
        const hiddenReviews = document.querySelectorAll('.hidden-review');
        const toShow = Array.from(hiddenReviews).slice(0, 10);

        toShow.forEach(r => r.classList.remove('hidden-review'));

        if (document.querySelectorAll('.hidden-review').length === 0) {
            this.style.display = 'none';
        }
    });

    // ⭐ INICIALIZAR ESTRELLAS DE REVIEW (si ya hay una review guardada)
    const ratingInput = document.getElementById("ratingInput");
    const selectedRating = parseInt(ratingInput?.value);
    const stars = document.querySelectorAll('#star-container .star');

    if (!isNaN(selectedRating)) {
        stars.forEach(star => {
            const value = parseInt(star.dataset.value);
            if (value <= selectedRating) {
                star.classList.add("selected");
            } else {
                star.classList.remove("selected");
            }
        });
    }

    // ⭐ CLICK PARA SELECCIONAR ESTRELLA
    stars.forEach(star => {
        star.addEventListener("click", function () {
            const rating = this.dataset.value;
            ratingInput.value = rating;

            stars.forEach(s => {
                s.classList.remove("selected");
                if (parseInt(s.dataset.value) <= rating) {
                    s.classList.add("selected");
                }
            });
        });
    });
});