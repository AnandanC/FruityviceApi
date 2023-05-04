
DROP TABLE IF EXISTS tblFruits;
CREATE TABLE IF NOT EXISTS tblFruits(
  FruitId      INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT
  , Genus      VARCHAR(32)
  ,Name        VARCHAR(64)
  ,Family      VARCHAR(32)
  ,OrderDesc   VARCHAR(32)
);

DROP TABLE IF EXISTS tblNutritions;
CREATE TABLE IF NOT EXISTS tblNutritions(
   NutritionId    INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT
  ,FruitId        INTEGER
  ,Carbohydrates  DECIMAL(5,2)
  ,Protein        DECIMAL(5,2)
  ,Fat            DECIMAL(5,2)
  ,Calories       DECIMAL(5,2)
  ,Sugar          DECIMAL(5,2)
);
