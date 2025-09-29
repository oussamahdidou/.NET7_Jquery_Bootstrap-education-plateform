$(document).ready(function () {
    $(".fa-bell").click(function () {
        $(".notifications-container").toggle();
    });
});
// $.ajax({
//     url: '/Dahboard/DashboardData', // Replace with your controller and action route
//     method: '',
//     data: { number: item }, // Send the number as a parameter
//     success: function (result) {
//         const Genders = result.map(obj => obj.gender);
//         const Counts = result.map(obj => obj.count);

//         console.log(Genders); // Output: ['Alice', 'Bob', 'Charlie']

//     },
//     error: function (error) {
//         console.log(error.status);

//     }
// });
document.addEventListener('DOMContentLoaded', function () {

    // Fetch data from the backend API
    fetch('/Dashboard/DashboardData') // Replace '/api/data' with the actual API endpoint URL
        .then(response => response.json())
        .then(data => {
            // Assuming the backend API returns an object with 'months' and 'values' properties
            var _genders = data.gender;
            var _Projects = data.result;
            const genders = _genders.map(obj => obj.gender);
            const counts = _genders.map(obj => obj.count);
            console.log(_genders);
            console.log(_Projects);
            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
            var values = [150, 220, 120, 180, 280, 240];

            // Create and render the charts
            var lineChartCanvas = document.getElementById('lineChart');
            var barChartCanvas = document.getElementById('barChart');
            var pieChartCanvas = document.getElementById('pieChart');
            var doughnutChartCanvas = document.getElementById('doughnutChart');

            var lineChart = createLineChart(lineChartCanvas, _Projects.map(obj => obj.month), _Projects.map(obj => obj.rowCount));
            var barChart = createBarChart(barChartCanvas, data.usersapp.map(obj => obj.user), data.usersapp.map(obj => obj.applicationcount));
            var pieChart = createPieChart(pieChartCanvas, genders, counts);
            var doughnutChart = createDoughnutChart(doughnutChartCanvas, data.response.map(obj => obj.name), data.response.map(obj => obj.likesnumber));
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });

    // Resize charts when window is resized
    // window.addEventListener('resize', function () {
    //     lineChart.resize();
    //     barChart.resize();
    //     pieChart.resize();
    //     doughnutChart.resize();
    // });

    // Create line chart
    function createLineChart(canvas, labels, data) {
        return new Chart(canvas, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Project Progression Over Time',
                    data: data,
                    borderColor: '#007bff',
                    fill: false,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true
            }
        });
    }

    // Create bar chart
    function createBarChart(canvas, labels, data) {
        return new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Top-Performing Students: Project Counts',
                    data: data,
                    backgroundColor: '#28a745'
                }]
            },
            options: {
                responsive: true,
                barPercentage: 0.6, // Adjust as needed for narrower bars
                categoryPercentage: 0.7, // Adjust as needed for narrower bars
                borderRadius: 10,
                borderRadius: 25,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // Create pie chart
    function createPieChart(canvas, labels, data) {
        return new Chart(canvas, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Gender Distribution',
                    data: data,
                    backgroundColor: ['#FFC0CB', '#AAAAFF']
                }]
            },
            options: {
                responsive: true
            }
        });
    }

    // Create doughnut chart
    function createDoughnutChart(canvas, labels, data) {
        return new Chart(canvas, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Most Liked Courses',
                    data: data,
                    backgroundColor: ['#f39c12', '#d35400', '#e74c3c', '#3498db', '#27ae60', '#8e44ad']
                }]
            },
            options: {
                responsive: true
            }
        });
    }

});

// Load the Google Charts library
// Access the data from ViewBag and serialize it to JSON
// Access data from ViewBag and assign it to a JavaScript variable

google.charts.load('current', { 'packages': ['geochart'] });
google.charts.setOnLoadCallback(drawGeoChart);

function drawGeoChart() {
    $.ajax({
        url: '/Dashboard/DashboardMap',
        dataType: 'json',
        success: function (data) {
            console.log(data)
            var geoData = new google.visualization.DataTable();
            geoData.addColumn('string', 'Country');
            geoData.addColumn('number', 'Popularity');

            for (var i = 0; i < data.length; i++) {
                geoData.addRow([data[i].nationality, data[i].count]);
            }

            var geoChart = new google.visualization.GeoChart(document.getElementById('geoChart'));

            geoChart.draw(geoData, {
                colorAxis: { colors: ['#Add8e6', '#0000ff'] } // Adjust colors as needed
            });
        }
    });
}