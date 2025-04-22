document.addEventListener("DOMContentLoaded", function () {
    const movieButtons = document.querySelectorAll(".movieBtn");

    movieButtons.forEach(button => {
        button.addEventListener("click", function () {
            const movieId = this.getAttribute("data-id");

            const hiddenInputs = document.querySelectorAll(".idFilm");
            hiddenInputs.forEach(input => {
                input.value = movieId;
            });
        });
    });
});