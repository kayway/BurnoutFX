-- --------------------------------------------------------
-- Host:                         192.168.1.150
-- Server version:               10.4.22-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             11.3.0.6337
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for burnoutfx
CREATE DATABASE IF NOT EXISTS `burnoutfx` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;
USE `burnoutfx`;

-- Dumping structure for procedure burnoutfx.initialize_player
DELIMITER //
CREATE PROCEDURE `initialize_player`(
	IN `player_id` VARCHAR(100),
	IN `player_name` VARCHAR(100)
)
BEGIN
INSERT IGNORE INTO `players`(`identifier`, `name`) VALUES(player_id, player_name);
SELECT `money`,`reputation`,`infamy` FROM `players` WHERE `identifier` = player_id;
END//
DELIMITER ;

-- Dumping structure for table burnoutfx.players
CREATE TABLE IF NOT EXISTS `players` (
  `identifier` varchar(100) NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `reputation` int(10) unsigned NOT NULL DEFAULT 0,
  `infamy` int(10) unsigned NOT NULL DEFAULT 0,
  `money` int(10) unsigned NOT NULL DEFAULT 1000,
  PRIMARY KEY (`identifier`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Dumping data for table burnoutfx.players: ~2 rows (approximately)
DELETE FROM `players`;
/*!40000 ALTER TABLE `players` DISABLE KEYS */;
INSERT INTO `players` (`identifier`, `name`, `reputation`, `infamy`, `money`) VALUES
	('8d4c68c598bbad3e6e3db61f48f08355a13fd369', 'Kayway', 0, 0, 1000),
	('d9a63f8ec6c3d3b0a3461fffbcca72639d9df41c', 'Tarith', 0, 0, 1000);
/*!40000 ALTER TABLE `players` ENABLE KEYS */;

-- Dumping structure for table burnoutfx.timetrials
CREATE TABLE IF NOT EXISTS `timetrials` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `track` varchar(100) NOT NULL,
  `time` int(10) unsigned NOT NULL,
  `vehicle` varchar(100) DEFAULT 'Car Not Found',
  `player_id` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

-- Dumping data for table burnoutfx.timetrials: ~3 rows (approximately)
DELETE FROM `timetrials`;
/*!40000 ALTER TABLE `timetrials` DISABLE KEYS */;
INSERT INTO `timetrials` (`id`, `track`, `time`, `vehicle`, `player_id`) VALUES
	(1, 'Sandy Airfield', 32196, 'Cheetah', '8d4c68c598bbad3e6e3db61f48f08355a13fd369'),
	(2, 'Sandy Airfield', 35658, 'NULL', '8d4c68c598bbad3e6e3db61f48f08355a13fd369'),
	(3, 'Sandy Airfield', 36624, 'Avarus', '8d4c68c598bbad3e6e3db61f48f08355a13fd369'),
	(4, 'Sandy Airfield', 32295, 'Penetrator', '8d4c68c598bbad3e6e3db61f48f08355a13fd369');
/*!40000 ALTER TABLE `timetrials` ENABLE KEYS */;

-- Dumping structure for table burnoutfx.tracks
CREATE TABLE IF NOT EXISTS `tracks` (
  `title` varchar(50) NOT NULL,
  `colour` int(3) unsigned NOT NULL DEFAULT 5,
  `icon` int(3) unsigned NOT NULL DEFAULT 315,
  `start` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL,
  `checkpoints` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL,
  `radius` float NOT NULL DEFAULT 24,
  `transparency` float NOT NULL DEFAULT 1,
  `enabled` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`title`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Dumping data for table burnoutfx.tracks: ~2 rows (approximately)
DELETE FROM `tracks`;
/*!40000 ALTER TABLE `tracks` DISABLE KEYS */;
INSERT INTO `tracks` (`title`, `colour`, `icon`, `start`, `checkpoints`, `radius`, `transparency`, `enabled`) VALUES
	('Cannonball Run', 5, 315, '{ "X": -1607.318359375, "Y": -959.67138671875, "Z": 12.597019195557, "Heading": 321.09140014648, "Type": 5 }', '[\r\n    {"X": -1460.4228515625, "Y":-781.34197998047,"Z": 23.361558914185,"Heading": 318.8801574707,"Type": 5 },\r\n    {"X": -1340.3503417969,"Y": -679.0263671875,"Z": 25.581365585327,"Heading": 306.97860717773,"Type": 5 },\r\n    {"X": -1243.9234619141,"Y": -566.95849609375,"Z": 28.111865997314,"Heading": 33.419361114502,"Type": 5 },\r\n    {"X": -1426.6506347656,"Y": -327.95690917969,"Z": 44.007438659668,"Heading": 39.909969329834,"Type": 5 },\r\n    {"X": -1583.845703125,"Y": -144.46768188477,"Z": 55.170845031738,"Heading": 34.813705444336,"Type": 5 },\r\n    {"X": -1895.6900634766,"Y": 176.14093017578,"Z": 81.726440429688,"Heading": 43.351215362549,"Type": 5 },\r\n    {"X": -1972.7966308594,"Y": 538.77508544922,"Z": 109.95614624023,"Heading": 349.78002929688,"Type": 5 },\r\n    {"X": -1733.6348876953,"Y": 828.24841308594,"Z": 142.94569396973,"Heading": 323.63955688477,"Type": 5 },\r\n    {"X": -1622.5186767578,"Y": 1139.5323486328,"Z": 150.15615844727,"Heading": 13.660859107971,"Type": 5 },\r\n    {"X": -1546.9471435547,"Y": 1407.9493408203,"Z": 124.12697601318,"Heading": 317.61862182617,"Type": 5 },\r\n    {"X": -1507.9011230469,"Y": 1683.7175292969,"Z": 98.48136138916,"Heading": 4.4252882003784,"Type": 5 },\r\n    {"X": -1449.0495605469,"Y": 1976.0850830078,"Z": 69.504554748535,"Heading": 29.093732833862,"Type": 5 },\r\n    {"X": -1505.7811279297,"Y": 2131.2114257813,"Z": 55.617652893066,"Heading": 329.19528198242,"Type": 5 },\r\n    {"X": -1425.1112060547,"Y": 2147.1608886719,"Z": 53.005302429199,"Heading": 269.25903320313,"Type": 5 },\r\n    {"X": -1356.0765380859,"Y": 2220.62109375,"Z": 48.389919281006,"Heading": 354.47640991211,"Type": 5 },\r\n    {"X": -1297.9562988281,"Y": 2488.7194824219,"Z": 21.916236877441,"Heading": 317.03799438477,"Type": 5 },\r\n    {"X": -1123.8458251953,"Y": 2657.6137695313,"Z": 17.203315734863,"Heading": 310.96087646484,"Type": 5 },\r\n    {"X": -859.53430175781,"Y": 2750.5139160156,"Z": 22.800928115845,"Heading": 274.66470336914,"Type": 5 },\r\n    {"X": -537.13549804688,"Y": 2846.5302734375,"Z": 33.907627105713,"Heading": 261.95040893555,"Type": 5 },\r\n    {"X": -209.5178527832,"Y": 2874.6220703125,"Z": 46.778469085693,"Heading": 248.77243041992,"Type": 5 },\r\n    {"X": 69.805358886719,"Y": 2732.6301269531,"Z": 55.677001953125,"Heading": 226.16117858887,"Type": 5 },\r\n    {"X": 331.44790649414,"Y": 2648.9692382813,"Z": 44.302200317383,"Heading": 287.69750976563,"Type": 5 },\r\n    {"X": 961.46356201172,"Y": 2693.6887207031,"Z": 39.790191650391,"Heading": 269.58987426758,"Type": 5 },\r\n    {"X": 1543.1647949219,"Y": 2775.83203125,"Z": 37.697772979736,"Heading": 303.06399536133,"Type": 5 },\r\n    {"X": 1975.9383544922,"Y": 2979.5173339844,"Z": 45.346519470215,"Heading": 281.96737670898,"Type": 5 },\r\n    {"X": 2295.8908691406,"Y": 2995.9904785156,"Z": 46.215560913086,"Heading": 250.00836181641,"Type": 5 },\r\n    {"X": 2470.4860839844,"Y": 2893.2915039063,"Z": 46.883125305176,"Heading": 320.52627563477,"Type": 5 },\r\n    {"X": 2715.75390625,"Y": 3212.0219726563,"Z": 53.770889282227,"Heading": 328.04644775391,"Type": 5 },\r\n    {"X": 2901.0354003906,"Y": 3659.1994628906,"Z": 52.231853485107,"Heading": 344.22540283203,"Type": 5 },\r\n    {"X": 2864.6066894531,"Y": 4240.138671875,"Z": 49.647491455078,"Heading": 17.717800140381,"Type": 5 },\r\n    {"X": 2584.5341796875,"Y": 5317.4731445313,"Z": 44.159732818604,"Heading": 16.238145828247,"Type": 5 },\r\n    {"X": 2307.0017089844,"Y": 5925.72265625,"Z": 48.012245178223,"Heading": 38.409969329834,"Type": 5 },\r\n    {"X": 2085.9475097656,"Y": 6110.4301757813,"Z": 50.005531311035,"Heading": 51.440216064453,"Type": 5 },\r\n    {"X": 1904.2946777344,"Y": 6344.375,"Z": 41.979625701904,"Heading": 65.717811584473,"Type": 5 },\r\n    {"X": 1512.3416748047,"Y": 6450.8530273438,"Z": 22.291770935059,"Heading": 66.489120483398,"Type": 5 },\r\n    {"X": 909.94793701172,"Y": 6494.9145507813,"Z": 20.853240966797,"Heading": 88.569923400879,"Type": 5 },\r\n    {"X": 389.83612060546,"Y": 6571.330078125,"Z": 26.932916641236,"Heading": 87.253578186036,"Type": 5 },\r\n    {"X": 127.84744262696,"Y": 6554.578125,"Z": 30.927782058716,"Heading": 43.02042388916,"Type": 5 },\r\n    {"X": 49.179161071778,"Y": 6634.1215820312,"Z": 30.892223358154,"Heading": 44.478343963624,"Type": 5 },\r\n    {"X": -30.021013259888,"Y": 6620.7524414062,"Z": 29.751941680908,"Heading": 125.5305480957,"Type": 9 }\r\n]', 24, 1, b'1'),
	('Sandy Airfield', 5, 315, '{ "X": 1718.0, "Y": 3254.5, "Z": 40.5, "Heading": 105.0, "Type": 5 }', '[                                                                                         \r\n    {"X": 1472.0715332032, "Y": 3188.8779296875, "Z": 39.729152679444, "Heading": 105.37201690674, "Type": 5},\r\n    {"X": 1073.3435058594, "Y": 3043.9104003906, "Z": 40.528339385986, "Heading": 197.2525177002, "Type": 5},\r\n    {"X": 1337.2255859375, "Y": 3082.1672363282, "Z": 39.850803375244, "Heading": 283.56066894532, "Type": 5},\r\n    {"X": 1598.4370117188, "Y": 3195.6440429688, "Z": 39.846961975098, "Heading": 130.9425354004, "Type": 9}\r\n]', 24, 1, b'1');
/*!40000 ALTER TABLE `tracks` ENABLE KEYS */;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
