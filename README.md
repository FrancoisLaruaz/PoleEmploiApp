# PoleEmploiApp
Application collecting job offers and exporting data via an Excel file.

L'application dispose d'un IHM avec une seule page. Cette page comporte :
- Un bouton pour rafraichir les données (offres d'emploi sur Paris, Rennes et Bordeaux).
- Un lien permettant de télécharger un fichier Excel avec les données enregistrées en base de données. Ce fichier Excel permettra à l'utilisateur de manipuler les données et trouver les statistiques dont il a besoin.

## Pré-requis

- Visual Studio 2019 ou 2022

## Démarrage

1) Ouvrez le fichier PoleEmploiApp.sln avec Visual Studio dans le répertoire PoleEmploiApp.
2) Lancez la solution dans Visual Studio

 

## [Démo](https://francoislaruaz.azurewebsites.net/) en ligne
https://francoislaruaz.azurewebsites.net/
Site hébergé sur Azure.
Pour cette demo, seules les offres de Paris, Bordeaux et Paris (75101) sont récupérés.

## Accès Base de données

Une base de données SQL Server est utilisée pour sauvegarder les informations. Un server AWS RDS est utilisé afin d'héberger la  base de données (https://aws.amazon.com/rds/). Le lien entre la BDD et l'application se fait via EF 6.0.
 Vous pouvez accéder à la base de données distante via SQL Server Management studio en utilisant les informations suivantes :
 
 - Host : database-1.cx5qrvj1ajmz.ca-central-1.rds.amazonaws.com
 - User Id : admin
 - Password : HelloWork

## Fabriqué avec

* .NET Core 6.0 (MVC)
* SQL Server Express 2019 + EF 6 
* Razor + jQuery pour le front

## Améliorations possibles

* Gestion des erreurs
* Utilisation de l'API de rccuperation des codes de ville au lieu d'avoir les codes en dur
* Meilleure utilisation du token d'authentification : Nous n'avons pas besoin d'un token pour chaque requête. Celui-ci doit etre réutilisé tant qu'il est valide (L'expiration du token est fournie lors de sa récupération). 

## Auteur

* **François Laruaz**
