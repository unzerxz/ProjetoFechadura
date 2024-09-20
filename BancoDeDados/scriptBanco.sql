-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema bdFechadura
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema bdFechadura
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `bdFechadura` DEFAULT CHARACTER SET utf8 ;
USE `bdFechadura` ;

-- -----------------------------------------------------
-- Table `bdFechadura`.`cargo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdFechadura`.`cargo` (
  `idCargo` INT NOT NULL AUTO_INCREMENT,
  `nomeCargo` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`idCargo`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `bdFechadura`.`perfil`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdFechadura`.`perfil` (
  `idPerfil` INT NOT NULL AUTO_INCREMENT,
  `tipoPerfil` VARCHAR(15) NOT NULL,
  PRIMARY KEY (`idPerfil`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `bdFechadura`.`funcionario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdFechadura`.`funcionario` (
  `idFuncionario` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(45) NOT NULL,
  `nomeUsuario` VARCHAR(45) NOT NULL,
  `credencialCartao` VARCHAR(20) NULL,
  `credencialTeclado` INT(6) NULL,
  `senha` VARCHAR(30) NOT NULL,
  `isAtivo` BIT(1) NOT NULL DEFAULT 0,
  `cargo_idCargo` INT NOT NULL,
  `perfil_idPerfil` INT NOT NULL,
  PRIMARY KEY (`idFuncionario`, `cargo_idCargo`, `perfil_idPerfil`),
  UNIQUE INDEX `credencialCartao_UNIQUE` (`credencialCartao` ASC) VISIBLE,
  UNIQUE INDEX `credencialTeclado_UNIQUE` (`credencialTeclado` ASC) VISIBLE,
  INDEX `fk_funcionario_cargo_idx` (`cargo_idCargo` ASC) VISIBLE,
  INDEX `fk_funcionario_perfil1_idx` (`perfil_idPerfil` ASC) VISIBLE,
  CONSTRAINT `fk_funcionario_cargo`
    FOREIGN KEY (`cargo_idCargo`)
    REFERENCES `bdFechadura`.`cargo` (`idCargo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_funcionario_perfil1`
    FOREIGN KEY (`perfil_idPerfil`)
    REFERENCES `bdFechadura`.`perfil` (`idPerfil`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `bdFechadura`.`sala`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdFechadura`.`sala` (
  `idSala` INT NOT NULL AUTO_INCREMENT,
  `identificacaoSala` VARCHAR(10) NOT NULL,
  `status` BIT(1) NOT NULL,
  `credencialSala` VARCHAR(20) NULL,
  `isAtivo` BIT(1) NOT NULL,
  `funcionario_idFuncionario` INT NOT NULL,
  PRIMARY KEY (`idSala`, `funcionario_idFuncionario`),
  INDEX `fk_sala_funcionario1_idx` (`funcionario_idFuncionario` ASC) VISIBLE,
  CONSTRAINT `fk_sala_funcionario1`
    FOREIGN KEY (`funcionario_idFuncionario`)
    REFERENCES `bdFechadura`.`funcionario` (`idFuncionario`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `bdFechadura`.`registro`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdFechadura`.`registro` (
  `idRegistro` INT NOT NULL AUTO_INCREMENT,
  `horarioEntrada` DATETIME NOT NULL,
  `horarioSaida` DATETIME NULL,
  `sala_idSala` INT NOT NULL,
  `funcionario_idFuncionario` INT NOT NULL,
  PRIMARY KEY (`idRegistro`, `sala_idSala`, `funcionario_idFuncionario`),
  INDEX `fk_registro_sala1_idx` (`sala_idSala` ASC) VISIBLE,
  INDEX `fk_registro_funcionario1_idx` (`funcionario_idFuncionario` ASC) VISIBLE,
  CONSTRAINT `fk_registro_sala1`
    FOREIGN KEY (`sala_idSala`)
    REFERENCES `bdFechadura`.`sala` (`idSala`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_registro_funcionario1`
    FOREIGN KEY (`funcionario_idFuncionario`)
    REFERENCES `bdFechadura`.`funcionario` (`idFuncionario`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `bdFechadura`.`tokenFuncionario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `bdFechadura`.`tokenFuncionario` (
  `idToken` INT NOT NULL AUTO_INCREMENT,
  `token` VARCHAR(500) NOT NULL,
  `timeExpiracao` DATETIME NOT NULL,
  `funcionario_idFuncionario` INT NOT NULL,
  PRIMARY KEY (`idToken`, `funcionario_idFuncionario`),
  INDEX `fk_tokenFuncionario_funcionario1_idx` (`funcionario_idFuncionario` ASC) VISIBLE,
  CONSTRAINT `fk_tokenFuncionario_funcionario1`
    FOREIGN KEY (`funcionario_idFuncionario`)
    REFERENCES `bdFechadura`.`funcionario` (`idFuncionario`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

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
    (`idFuncionario`, `nome`, `nomeUsuario`, `credencialCartao`, `credencialTeclado`, `senha`, `isAtivo`, `cargo_idCargo`, `perfil_idPerfil`)
VALUES 
    (1, 'sentinela', 'sentinela', NULL, NULL, 'senha', 1, 1, 1),
    (2, 'admin', 'admin', NULL, NULL, '123456', 1, 1, 3);
