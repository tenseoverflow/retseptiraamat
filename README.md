# Retseptiraamat

Retseptiraamat on veebirakendus, mis aitab hallata ja jagada retsepte ning hoida silma peal külmkapis olevatel toiduainetel.

## Paigaldusjuhend

### Paigaldamine ja käivitamine productionis

1. Muuda `sample.env` fail nimeks `.env` ja muuda seal vajalikud keskkonnamuutujad vastavalt oma vajadustele.

2. Kui kasutad NGINX reverse proxyna, siis muuda `sample.nginx.conf` parameetreid vastavalt oma vajadustele ja vii fail nginx configide kausta.

3. Ehita projekt:

   ```bash
    docker compose --profile prod build
   ```

4. Käivita projekt
   ```bash
    docker compose --profile prod up
   ```

### Paigaldamine ja käivitamine arendajakeskkonnas

1. Muuda `sample.env` fail nimeks `.env` ja muuda seal vajalikud keskkonnamuutujad vastavalt oma vajadustele.

2. Ehita projekt:

   ```bash
   docker compose --profile dev build
   ```

3. Käivita projekt
   ```bash
   docker compose --profile dev up
   ```
4. Ava oma brauser ja mine aadressile `http://localhost:5173`.
