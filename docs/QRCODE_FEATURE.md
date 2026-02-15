# Fonctionnalité QR Code pour les Équipements

## Description
La fonctionnalité QR Code permet de générer un code QR unique pour chaque équipement. Ce code peut être imprimé et apposé sur l'équipement physique pour permettre un accès rapide aux informations de l'équipement via un mobile.

## Modifications Apportées

### 1. Installation de QRCoder
Package NuGet `QRCoder v1.7.0` ajouté au projet pour la génération de codes QR.

### 2. Service QRCodeService (Application/Services/QRCodeService.cs)
Un nouveau service responsable de la génération des QR codes en format SVG :
- **Génération SVG**: Crée des codes QR au format SVG (scalable et imprimable)
- **Classe ECCLevel.Q**: Utilise le niveau de correction d'erreur Q (25% d'erreur corrigible)
- **Gestion d'erreurs**: Capture et rapporte les erreurs de génération

### 3. Composant QRCodeComponent (Web/Features/Equipment/Components/QRCodeComponent.razor)
Un composant Blazor réutilisable qui :
- Génère automatiquement le QR code au chargement
- Affiche le code QR en SVG
- Fournit un bouton d'impression
- S'adapte responsément pour l'affichage et l'impression

**URL du QR code** : `/equipment/view/{EquipmentId}`

### 4. Page EquipmentView (Web/Features/Equipment/Pages/EquipmentView.razor)
Une nouvelle page permettant de visualiser les détails complets d'un équipement :
- **URL** : `/equipment/view/{id}`
- Affiche l'image de l'équipement
- Affiche toutes les informations (ID, Serial, Localisation, Date d'achat)
- Affiche l'historique des interventions
- Affiche le QR code avec bouton d'impression dans une barre latérale
- Page simple et optimisée pour les smartphones

### 5. Intégration dans EquipmentEdit
La page d'édition d'équipement affiche maintenant le QR code en barre latérale (uniquement lors de l'édition, pas lors de la création)

### 6. Intégration dans EquipmentList
Un nouveau bouton "Voir détails & QR" permet d'accéder rapidement à la page de détails et au QR code

## Flux d'Utilisation

### Pour les Utilisateurs
1. **Naviguer vers un équipement** :
   - Depuis la liste, cliquer sur "Voir détails & QR" ou sur l'image
   - Ou accéder directement via l'URL `/equipment/view/{id}`

2. **Visualiser le QR code** :
   - Le QR code s'affiche dans la barre latérale
   - Cliquer sur "🖨️ Imprimer QR Code" pour imprimer

3. **Scanner le QR code** :
   - Utiliser un téléphone pour scanner
   - Le téléphone est redirigé vers `/equipment/view/{id}`
   - Affichage des détails et de l'historique

### Pour les Développeurs

**Générer un QR code** :
```csharp
var qrCodeSvg = qrCodeService.GenerateQRCodeSvg("https://example.com/equipment/view/1");
```

**Utiliser le composant** :
```razor
<QRCodeComponent EquipmentId="@equipmentId" EquipmentName="@equipmentName" />
```

## Enregistrement du Service
Le service `QRCodeService` a été enregistré dans `Program.cs`:
```csharp
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
```

## Caractéristiques Techniques

### Format SVG
- Scalable sans perte de qualité
- Facilement personnalisable via CSS
- Taille de fichier réduite
- Support d'impression excellent

### Niveau de Correction d'Erreur
- **Niveau Q** : Peut corriger jusqu'à 25% du code s'il est endommagé
- Équilibre entre capacité de données et fiabilité

### URL Codée
L'URL codée dans le QR code inclut le domaine complet :
```
https://mon-app.com/equipment/view/42
```

## Impression

### Via le Navigateur
L'impression utilise la fonctionnalité print du navigateur :
- Utilisateurs mobiles : Option "Imprimer" du navigateur
- Utilisateurs desktop : Ctrl+P ou Cmd+P

### Styling Print
CSS spécifique pour l'impression :
- Les boutons sont masqués (@media print)
- Le QR code est centré
- Taille adaptée pour l'impression

## Exemple pratique

**Cas d'usage** :
1. Vous avez un équipement industriel en atelier
2. Vous générez et imprimez son QR code
3. Vous collez le QR code sur l'équipement
4. N'importe quel technicien peut scanner et voir :
   - Les détails de l'équipement
   - L'historique des interventions
   - Le lien pour l'éditer

## Notes de Performance
- La génération SVG est rapide et efficace
- Pas de limite d'équipements
- Chaque QR code est généré à la demande
- Cache du navigateur appliqué

## Limitations et Futures Améliorations

### Actuellement
- QR codes générés en SVG (vectoriel)
- Impression via navigateur uniquement
- URL pointant vers page web

### Améliorations Possibles
- Génération d'image PNG pour emailing
- API pour télécharger le QR code en image
- Impression directe depuis le serveur
- QR codes avec logo personnalisé
- Historique de modifications accessible depuis le scan
