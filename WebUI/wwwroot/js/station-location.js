export function getCurrentPosition() {
    return new Promise((resolve) => {
        if (!navigator.geolocation) {
            resolve({ error: "Location is not available in this browser. Showing configured stations." });
            return;
        }

        navigator.geolocation.getCurrentPosition(
            ({ coords }) => resolve({ latitude: coords.latitude, longitude: coords.longitude }),
            () => resolve({ error: "Location permission was not granted. Showing configured stations." }),
            { enableHighAccuracy: false, timeout: 10000, maximumAge: 300000 }
        );
    });
}