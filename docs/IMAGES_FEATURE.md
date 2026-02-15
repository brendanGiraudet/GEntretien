# Fonctionnalité d'Images pour les Équipements

## Description
La fonctionnalité d'images permet d'assigner une image à chaque équipement. Cela est utile pour identifier visuellement les équipements et avoir une référence visuelle rapide.

## Modifications Apportées

### 1. Entité Equipment (Domain/Entities/Equipment.cs)
Ajout de trois propriétés pour gérer les images :
- `ImageData` (byte[]?): Contient les données binaires de l'image
- `ImageFileName` (string?): Stocke le nom original du fichier
- `ImageContentType` (string?): Stocke le type MIME de l'image (e.g., image/jpeg, image/png)

### 2. Migration Base de Données
Une migration EF Core a été créée (`AddImageToEquipment`) qui ajoute les colonnes suivantes à la table `Equipments`:
- `ImageContentType` (TEXT NULL)
- `ImageData` (BLOB NULL)
- `ImageFileName` (TEXT NULL)

### 3. Service ImageService (Application/Services/ImageService.cs)
Un nouveau service responsable du traitement des fichiers image avec:
- **Validation du type d'extension**: Accepte uniquement .jpg, .jpeg, .png, .gif, .webp
- **Validation du type MIME**: Vérifie que le fichier est vraiment une image
- **Limitation de taille**: Par défaut 5MB maximum
- **Lecture en mémoire**: Convertit le fichier en tableau d'octets pour stockage en DB

Types acceptés :
- image/jpeg
- image/png
- image/gif
- image/webp

### 4. Composant ImageUploadComponent (Web/Features/Equipment/Components/ImageUploadComponent.razor)
Un composant Blazor réutilisable qui fournit:
- Un champ input pour sélectionner une image
- Affichage de l'image actuelle avec aperçu
- Bouton pour supprimer l'image
- Messages de statut et gestion des erreurs
- Validation côté client

### 5. Page EquipmentEdit Mise à Jour
Intégration du composant ImageUploadComponent dans le formulaire d'édition d'équipement:
```razor
<ImageUploadComponent 
    ImageData="_model.ImageData"
    ImageFileName="_model.ImageFileName"
    ImageContentType="_model.ImageContentType"
    OnImageChanged="HandleImageChanged" />
```

### 6. Page EquipmentList Mise à Jour
Affichage d'une thumbnail de l'image dans la liste des équipements:
- Images affichées comme des thumbnails (max 50x50px)
- Utilise l'attribut `src="data:..."` pour afficher l'image en base64
- Affiche un tiret (—) si aucune image n'est présente

## Enregistrement du Service
Le service `ImageService` a été enregistré dans `Program.cs`:
```csharp
builder.Services.AddScoped<IImageService, ImageService>();
```

## Utilisation

### Pour Ajouter une Image à un Équipement:
1. Naviguez vers la page "Equipements"
2. Cliquez sur "Créer un équipement" ou "Éditer" un équipement existant
3. Remplissez les champs habituels (Nom, Serial, Location)
4. Utilisez le champ "Image" pour sélectionner une image depuis votre ordinateur
5. L'image sera validée et affichée en aperçu
6. Cliquez sur "Enregistrer"

### Pour Supprimer une Image:
1. Depuis la page d'édition d'un équipement
2. Un bouton "Supprimer l'image" est disponible si une image existe
3. Cliquez sur le bouton pour supprimer l'image
4. Cliquez sur "Enregistrer" pour confirmer

## Limitation de Taille de Fichier
Par défaut : **5 MB** maximum

Pour modifier cette limite, changez le paramètre `maxFileSize` lors de l'appel à `ProcessImageFileAsync()`.

## Gestion des Erreurs
Le composant affiche des messages d'erreur lisibles :
- Fichier trop volumineux
- Type de fichier non autorisé
- Type MIME invalide
- Erreurs de traitement du fichier

## Considérations de Performance
- Les images sont stockées en base de données sous forme BLOB
- Chaque image est convertie en base64 pour l'affichage dans le navigateur
- Les thumbnails (50x50px) utilisent le même contenu image que l'original
- Pour les applications à très grande échelle, envisagez un système de stockage externe (cloud storage, filesystem)

## Format des Données
Les images sont stockées comme :
- `ImageData`: Tableau d'octets bruts de l'image
- `ImageContentType`: Type MIME pour la reconstruction correcte (data:image/jpeg;base64,...)
- `ImageFileName`: Nom du fichier pour référence
