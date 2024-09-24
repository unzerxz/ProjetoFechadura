-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema bdFechadura
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `bdFechadura` DEFAULT CHARACTER SET utf8 ;
-- -----------------------------------------------------
-- Schema bdfechadura
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Table `bdfechadura`.`cargo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdfechadura`.`cargo` (
  `idCargo` INT NOT NULL AUTO_INCREMENT,
  `nomeCargo` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`idCargo`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `bdfechadura`.`perfil`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdfechadura`.`perfil` (
  `idPerfil` INT NOT NULL AUTO_INCREMENT,
  `tipoPerfil` VARCHAR(15) NOT NULL,
  PRIMARY KEY (`idPerfil`))
ENGINE = InnoDB
AUTO_INCREMENT = 4
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `bdfechadura`.`funcionario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdfechadura`.`funcionario` (
  `idFuncionario` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(45) NOT NULL,
  `nomeUsuario` VARCHAR(45) NOT NULL,
  `email` VARCHAR(255) NOT NULL,
  `credencialCartao` VARCHAR(20) NULL DEFAULT NULL,
  `credencialTeclado` INT NULL DEFAULT NULL,
  `senha` VARCHAR(30) NOT NULL,
  `isAtivo` BIT(1) NOT NULL DEFAULT b'0',
  `cargo_idCargo` INT NOT NULL,
  `perfil_idPerfil` INT NOT NULL,
  PRIMARY KEY (`idFuncionario`, `cargo_idCargo`, `perfil_idPerfil`),
  UNIQUE INDEX `credencialCartao_UNIQUE` (`credencialCartao` ASC) VISIBLE,
  UNIQUE INDEX `credencialTeclado_UNIQUE` (`credencialTeclado` ASC) VISIBLE,
  INDEX `fk_funcionario_cargo_idx` (`cargo_idCargo` ASC) VISIBLE,
  INDEX `fk_funcionario_perfil1_idx` (`perfil_idPerfil` ASC) VISIBLE,
  UNIQUE INDEX `email_UNIQUE` (`email` ASC) VISIBLE,
  CONSTRAINT `fk_funcionario_cargo`
    FOREIGN KEY (`cargo_idCargo`)
    REFERENCES `bdfechadura`.`cargo` (`idCargo`),
  CONSTRAINT `fk_funcionario_perfil1`
    FOREIGN KEY (`perfil_idPerfil`)
    REFERENCES `bdfechadura`.`perfil` (`idPerfil`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `bdfechadura`.`sala`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdfechadura`.`sala` (
  `idSala` INT NOT NULL AUTO_INCREMENT,
  `identificacaoSala` VARCHAR(10) NOT NULL,
  `status` BIT(1) NOT NULL,
  `credencialSala` VARCHAR(20) NULL DEFAULT NULL,
  `isAtivo` BIT(1) NOT NULL,
  `funcionario_idFuncionario` INT NOT NULL,
  PRIMARY KEY (`idSala`, `funcionario_idFuncionario`),
  INDEX `fk_sala_funcionario1_idx` (`funcionario_idFuncionario` ASC) VISIBLE,
  CONSTRAINT `fk_sala_funcionario1`
    FOREIGN KEY (`funcionario_idFuncionario`)
    REFERENCES `bdfechadura`.`funcionario` (`idFuncionario`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `bdfechadura`.`registro`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdfechadura`.`registro` (
  `idRegistro` INT NOT NULL AUTO_INCREMENT,
  `horarioEntrada` DATETIME NOT NULL,
  `horarioSaida` DATETIME NULL DEFAULT NULL,
  `sala_idSala` INT NOT NULL,
  `funcionario_idFuncionario` INT NOT NULL,
  PRIMARY KEY (`idRegistro`, `sala_idSala`, `funcionario_idFuncionario`),
  INDEX `fk_registro_sala1_idx` (`sala_idSala` ASC) VISIBLE,
  INDEX `fk_registro_funcionario1_idx` (`funcionario_idFuncionario` ASC) VISIBLE,
  CONSTRAINT `fk_registro_funcionario1`
    FOREIGN KEY (`funcionario_idFuncionario`)
    REFERENCES `bdfechadura`.`funcionario` (`idFuncionario`),
  CONSTRAINT `fk_registro_sala1`
    FOREIGN KEY (`sala_idSala`)
    REFERENCES `bdfechadura`.`sala` (`idSala`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `bdfechadura`.`tokenfuncionario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdfechadura`.`tokenfuncionario` (
  `idToken` INT NOT NULL AUTO_INCREMENT,
  `token` VARCHAR(500) NOT NULL,
  `timeExpiracao` DATETIME NOT NULL,
  `funcionario_idFuncionario` INT NOT NULL,
  PRIMARY KEY (`idToken`, `funcionario_idFuncionario`),
  INDEX `fk_tokenFuncionario_funcionario1_idx` (`funcionario_idFuncionario` ASC) VISIBLE,
  CONSTRAINT `fk_tokenFuncionario_funcionario1`
    FOREIGN KEY (`funcionario_idFuncionario`)
    REFERENCES `bdfechadura`.`funcionario` (`idFuncionario`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

USE `bdfechadura`;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

-- Insere o cargo 'indefinido' com id 1
INSERT INTO `bdFechadura`.`cargo` (`idCargo`, `nomeCargo`)
VALUES (1, 'indefinido');

-- Insere o perfil 'indefinido' com id 1
INSERT INTO `bdFechadura`.`perfil` (`idPerfil`, `tipoPerfil`)
VALUES (1, 'indefinido'), (2, 'usuario'), (3, 'administrador');

-- Insere o funcionario 'sentinela' com id 1
INSERT INTO `bdFechadura`.`funcionario` 
    (`idFuncionario`, `nome`, `nomeUsuario`, `email`, `credencialCartao`, `credencialTeclado`, `senha`, `isAtivo`, `cargo_idCargo`, `perfil_idPerfil`)
VALUES 
    (1, 'sentinela', 'sentinela', 'sentinela@gmail.com', NULL, NULL, 'senha', 1, 1, 1),
    (2, 'admin_admin', 'admin', 'admin@gmail.com', NULL, NULL, '123456', 1, 1, 3);