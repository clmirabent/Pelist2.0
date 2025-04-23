document.addEventListener("DOMContentLoaded", function () {
    const movieButtons = document.querySelectorAll(".movieBtn");

    movieButtons.forEach(button => {
        button.addEventListener("click", function () {

            const typeList = this.getAttribute("data-list-type");

            const listButtons = document.querySelectorAll(".listButton");

            const listTypes = {
                "1": "Pendiente",
                "2": "Completada",
                "3": "Favorita",
                "4": "Abandonada"
            };

            const typeListString = listTypes[typeList];

            listButtons.forEach(btn => {
                btn.style.backgroundColor = "";

                const originalText = btn.value;
                const iconHTML = btn.querySelector("i").outerHTML;
                btn.innerHTML = `${iconHTML} ${originalText}`;

                if (btn.value == typeListString) {
                    btn.style.backgroundColor = "red";
                    btn.innerHTML = `${iconHTML} Eliminar de ${originalText}`;
                }
            });

            const movieId = this.getAttribute("data-id");

            const hiddenInputs = document.querySelectorAll(".idFilm");
            hiddenInputs.forEach(input => {
                input.value = movieId;
            });
        });
    });
});


