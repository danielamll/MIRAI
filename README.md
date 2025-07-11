## 🔄 Cómo hacer un Commit en GitHub

Sigue estos pasos para realizar un commit y subir tus cambios a un repositorio en GitHub desde la terminal:

### 1. Verifica el estado de tus archivos
```bash
git status
```

### 2. Agrega los archivos que deseas incluir en el commit
Para agregar todos los archivos modificados:
```bash
git add .
```

O agrega archivos específicos:
```bash
git add nombre_del_archivo.ext
```

### 3. Realiza el commit con un mensaje descriptivo
```bash
git commit -m "Tu mensaje claro y conciso sobre los cambios realizados"
```

### 4. Sube tus cambios al repositorio en GitHub
```bash
git push origin main
```
> Asegúrate de que `main` sea el nombre correcto de tu rama. En algunos casos puede ser `master` u otro.
