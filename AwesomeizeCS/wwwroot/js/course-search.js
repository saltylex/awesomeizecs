document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('courseSearch');

    searchInput.addEventListener('keyup', function (e) {
        const searchTerm = e.target.value.toLowerCase();
        const courses = document.querySelectorAll('.course-block');

        courses.forEach(function (course) {
            const text = course.textContent.toLowerCase();
            const isVisible = text.includes(searchTerm);
            course.style.display = isVisible ? 'block' : 'none';
        });
    });
});
