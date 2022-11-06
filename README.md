# Forester

# Publikowanie nowej wersji

Wymagane są dwie rzeczy:
1. Build Forestera
2. Build Updatera

Aby przesłać Forestera na serwer trzeba usunąć nastepujące pliki:
- Apps
- Wszystkie configi w tym: libraryConfig.json, dataConfig.json
- Wszystkie foldery które są puste

następnie zaznaczyć wszystkie pliki w środku i zapisac je do zipa. Nie można zrobić tego z folderem!

Tak przygotowany .zip należy przesłać na serwer tą komendą:
POST
http://***REMOVED***:5000/api/Update/Forester/UploadNewVersion?version=2.0v

gdzie version oznacza wersje ofc

Aby przesłać Forestera na Google należy !wyczyścić!:
- Apps
- Wszystkie configi w tym: libraryConfig.json, dataConfig.json
- Wszystkie foldery które są puste

Dodać do niego folder z updaterem
Tak przygotowany folder zazipować i przesłać
