using System;
using System.Collections.Generic;
namespace RepoWebListener;

public class Dictionaries
{
 public static readonly Dictionary<string, string> ItemPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{   {"Item Cart Cannon", "items/Item Cart Cannon"},
    {"Item Cart Laser", "items/Item Cart Laser"},
    {"Item Cart Medium", "items/Item Cart Medium"},
    {"Item Cart Small", "items/Item Cart Small"},
    {"Item Drone Battery", "items/Item Drone Battery"},
    {"Item Drone Feather", "items/Item Drone Feather"},
    {"Item Drone Indestructible", "items/Item Drone Indestructible"},
    {"Item Drone Torque", "items/Item Drone Torque"},
    {"Item Drone Zero Gravity", "items/Item Drone Zero Gravity"},
    {"Item Extraction Tracker", "items/Item Extraction Tracker"},
    {"Item Grenade Duct Taped", "items/Item Grenade Duct Taped"},
    {"Item Grenade Explosive", "items/Item Grenade Explosive"},
    {"Item Grenade Human", "items/Item Grenade Human"},
    {"Item Grenade Shockwave", "items/Item Grenade Shockwave"},
    {"Item Grenade Stun", "items/Item Grenade Stun"},
    {"Item Gun Handgun", "items/Item Gun Handgun"},
    {"Item Gun Shotgun", "items/Item Gun Shotgun"},
    {"Item Gun Tranq", "items/Item Gun Tranq"},
    {"Item Health Pack Large", "items/Item Health Pack Large"},
    {"Item Health Pack Medium", "items/Item Health Pack Medium"},
    {"Item Health Pack Small", "items/Item Health Pack Small"},
    {"Item Melee Baseball Bat", "items/Item Melee Baseball Bat"},
    {"Item Melee Frying Pan", "items/Item Melee Frying Pan"},
    {"Item Melee Inflatable Hammer", "items/Item Melee Inflatable Hammer"},
    {"Item Melee Sledge Hammer", "items/Item Melee Sledge Hammer"},
    {"Item Melee Sword", "items/Item Melee Sword"},
    {"Item Mine Explosive", "items/Item Mine Explosive"},
    {"Item Mine Shockwave", "items/Item Mine Shockwave"},
    {"Mine Stun", "items/Item Mine Stun"},
    {"Item Orb Zero Gravity", "items/Item Orb Zero Gravity"},
    {"Item Power Crystal", "items/Item Power Crystal"},
    {"Item Duck Bucket", "items/Item Duck Bucket"},    
    {"Item Stun Baton", "items/Item Stun Baton"},
    {"Item Phase Bridge", "items/Item Phase Bridge"},
    {"Item Rubber Duck", "items/Item Rubber Duck"},    
    {"Item Upgrade Map Player Count", "items/Item Upgrade Map Player Count"},
    {"Item Upgrade Player Energy", "items/Item Upgrade Player Energy"},
    {"Item Upgrade Player Extra Jump", "items/Item Upgrade Player Extra Jump"},
    {"Item Upgrade Player Grab Range", "items/Item Upgrade Player Grab Range"},
    {"Item Upgrade Player Grab Strength", "items/Item Upgrade Player Grab Strength"},
    {"Item Upgrade Player Grab Throw", "items/Item Upgrade Player Grab Throw"},
    {"Item Upgrade Player Health", "items/Item Upgrade Player Health"},
    {"Item Upgrade Player Sprint Speed", "items/Item Upgrade Player Sprint Speed"},
    {"Item Upgrade Player Tumble Launch", "items/Item Upgrade Player Tumble Launch"},
    {"Item Upgrade Player Crouch Rest", "items/Item Upgrade Player Crouch Rest"},
    {"Item Upgrade Player Tumble Wings", "items/Item Upgrade Player Tumble Wings"},
    { "Item Valuable Tracker", "items/Item Valuable Tracker"}
};

public static readonly Dictionary<string, string> ValuablePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    
//Tiny Generic
    { "Valuable Diamond", "valuables/01 tiny/Valuable Diamond"},
    {"Valuable Emerald Bracelet", "valuables/01 tiny/Valuable Emerald Bracelet"},
    {"Valuable Goblet", "valuables/01 tiny/Valuable Goblet"},
    {"Valuable Ocarina", "valuables/01 tiny/Valuable Ocarina"},
    {"Valuable Pocket Watch", "valuables/01 tiny/Valuable Pocket Watch"},
    {"Valuable Uranium Mug", "valuables/01 tiny/Valuable Uranium Mug"},
    //Tiny Museum
    {"Valuable Cool Brain", "valuables/01 tiny/Valuable Cool Brain"},
    {"Valuable Fish", "valuables/01 tiny/Valuable Fish"},
    {"Valuable Golden Fish", "valuables/01 tiny/Valuable GoldFish"},
    {"Valuable Tooth", "valuables/01 tiny/Valuable Tooth"},
    {"Valuable Golden Tooth", "valuables/01 tiny/Valuable GoldTooth"},
    {"Valuable Ruben Doll", "valuables/01 tiny/Valuable RubenDoll"},
    {"Valuable Silver Fish", "valuables/01 tiny/Valuable SilverFish"},
    {"Valuable Toast", "valuables/01 tiny/Valuable Toast"},
    {"Valuable Toy Car", "valuables/01 tiny/Valuable Toy Car"},    
    //Small Generic
    { "Valuable Crown", "valuables/02 small/Valuable Crown"},
    {"Valuable Doll", "valuables/02 small/Valuable Doll"},
    {"Valuable Frog", "valuables/02 small/Valuable Frog"},
    {"Valuable Gem Box", "valuables/02 small/Valuable Gem Box"},
    {"Valuable Globe", "valuables/02 small/Valuable Globe"},
    {"Valuable Money", "valuables/02 small/Valuable Money"},
    {"Valuable Toy Monkey", "valuables/02 small/Valuable Toy Monkey"},
    {"Valuable Uranium Plate", "valuables/02 small/Valuable Uranium Plate"},
    {"Valuable Vase Small", "valuables/02 small/Valuable Vase Small"},
    //Small Arctic
    { "Valuable Arctic Bonsai", "valuables/02 small/Valuable Arctic Bonsai"},
    {"Valuable Arctic HDD", "valuables/02 small/Valuable Arctic HDD"},
    //Small Wizard
    {"Valuable Chomp Book", "valuables/02 small/Valuable Chomp Book"},
    {"Valuable Love Potion", "valuables/02 small/Valuable Love Potion"},
    //Small Manor
    {"Valuable Music Box", "valuables/02 small/Valuable Music Box"},
    //Small Museum
    {"Valuable Cubic Tower", "valuables/02 small/ValuableCubic Tower"},
    {"Valuable Flesh Blob", "valuables/02 small/Valuable Flesh Blob"},
    {"Valuable Flesh Blob", "valuables/02 small/Valuable Wire Figure"},
    //Medium Generic
    { "Valuable Bottle", "valuables/03 medium/Valuable Bottle"},
    {"Valuable Clown", "valuables/03 medium/Valuable Clown"},
    {"Valuable Vase", "valuables/03 medium/Valuable Vase"},
    {"Valuable Trophy", "valuables/03 medium/Valuable Trophy"},
    //Medium Arctic
    { "Valuable Arctic 3D Printer", "valuables/03 medium/Valuable Arctic 3D Printer"},
    {"Valuable Arctic Laptop", "valuables/03 medium/Valuable Arctic Laptop"},
    {"Valuable Arctic Propane Tank", "valuables/03 medium/Valuable Arctic Propane Tank"},
    {"Valuable Arctic Sample Six Pack", "valuables/03 medium/Valuable Arctic Sample Six Pack"},
    {"Valuable Arctic Sample", "valuables/03 medium/Valuable Bottle"},
    {"Valuable Computer", "valuables/03 medium/Valuable Computer"},
    {"Valuable Fan", "valuables/03 medium/Valuable Fan"},
    //Medium Wizard
    {"Valuable Wizard Goblin Head", "valuables/03 medium/Valuable Wizard Goblin Head"},
    {"Valuable Wizard Power Crystal", "valuables/03 medium/Valuable Wizard Power Crystal"},
    {"Valuable Wizard Time Glass", "valuables/03 medium/Valuable Wizard Time Glass"},    
    //Medium Manor
    { "Valuable Gramophone", "valuables/03 medium/Valuable Gramophone"},
    {"Valuable Radio", "valuables/03 medium/Valuable Radio"},
    {"Valuable Ship In Bottle", "valuables/03 medium/Valuable Ship in a bottle"},
    //Medium Museum
    {"Valuable MonkeyBox", "valuables/03 medium/Valuable Monkey Box"},
    {"Valuable Pacifier", "valuables/03 medium/Valuable Pacifier"},
    {"Valuable HandFace", "valuables/03 medium/Valuable HandFace"},
    {"Valuable Gumball Machine", "valuables/03 medium/Valuable Gumball"},
    {"Valuable Gumball Machine", "valuables/03 medium/Valuable Gumball"},                
    //Big Generic
    { "Valuable Vase Big", "valuables/04 big/Valuable Vase Big"},    
    //Big Arctic
    { "Valuable Arctic Barrel", "valuables/04 big/Valuable Arctic Barrel"},
    {"Valuable Arctic Big Sample", "valuables/04 big/Valuable Arctic Big Sample"},
    {"Valuable Arctic Creature Leg", "valuables/04 big/Valuable Arctic Creature Leg"},
    {"Valuable Arctic Flamethrower", "valuables/04 big/Valuable Arctic Flamethrower"},
    {"Valuable Arctic Guitar", "valuables/04 big/Valuable Arctic Guitar"},
    {"Valuable Arctic Sample Cooler", "valuables/04 big/Valuable Arctic Sample Cooler"},
    {"Valuable Ice Saw", "valuables/04 big/Valuable Ice Saw"},
    //Big Wizard
    {"Valuable Wizard Cube Of Knowledge", "valuables/04 big/Valuable Wizard Cube of Knowledge"},
    {"Valuable Wizard Master Potion", "valuables/04 big/Valuable Wizard Master Potion"},    
    //Big Manor
    {"Valuable Diamond Display", "valuables/04 big/Valuable Diamond Display"},
    {"Valuable Scream Doll", "valuables/04 big/Valuable Scream Doll"},
    {"Valuable Television", "valuables/04 big/Valuable Television"},
    //Big Museum
    {"Valuable Uranium Mug Deluxe", "valuables/04 big/Valuable Uranium Mug Deluxe"},
    {"Valuable Museum Boombox", "valuables/04 big/Valuable Uranium Museum Boombox"},
    {"Valuable Uranium Golden Poop", "valuables/04 big/Valuable Uranium Golden Swirl"},
    {"Valuable Uranium Gem Burger", "valuables/04 big/Valuable Uranium Gem Burger"},
    {"Valuable Uranium Egg", "valuables/04 big/Valuable Uranium Egg"},
    {"Valuable Uranium Baby Head", "valuables/04 big/Valuable Uranium Baby Head"},
    //Wide Generic
    { "Valuable Animal Crate", "valuables/05 wide/Valuable Animal Crate"},    
    //Wide Arctic
    {"Valuable Arctic Ice Block", "valuables/05 wide/Valuable Arctic Ice Block"},       
    //Wide Wizard
    {"Valuable Wizard Griffin Statue", "valuables/05 wide/Valuable Wizard Griffin Statue"},
    //Wide Manor
    {"Valuable Dinosaur", "valuables/05 wide/Valuable Dinosaur"},
    {"Valuable Piano", "valuables/05 wide/Valuable Piano"},
    //Wide Museum
    {"Valuable Horse", "valuables/06 tall/Valuable Horse"},
    //Tall Generic
    //Tall Arctic 
    {"Valuable Arctic Science Station", "valuables/06 tall/Valuable Arctic Science Station"},
    //Tall Wizard
    {"Valuable Wizard Dumgolf's Staff", "valuables/06 tall/Valuable Wizard Dumgolfs Staff"},
    {"Valuable Wizard Sword", "valuables/06 tall/Valuable Wizard Sword"},
    //Tall Manor
    {"Valuable Painting", "valuables/06 tall/Valuable Painting"},
    {"Valuable Harp", "valuables/06 tall/Valuable Harp"},
    //Tall Museum
    {"Valuable Milk", "valuables/06 tall/Valuable Milk"},
    //Very Tall Generic
    //Very Tall Arctic
    { "Valuable Arctic Server Rack", "valuables/07 very tall/Valuable Arctic Server Rack"},
    //Very Tall Wizard
    {"Valuable Wizard Broom", "valuables/07 very tall/Valuable Wizard Broom"},          
    //Very Tall Manor
    {"Valuable Grandfather Clock", "valuables/07 very tall/Valuable Grandfather Clock"},
    {"Valuable Golden Statue", "valuables/07 very tall/Valuable Golden Statue"},         
    //Very Tall Museum
    {"Valuable Traffic light", "valuables/07 very tall/Valuable Traffic Light"},
    {"Valuable Blender", "valuables/07 very tall/Valuable Blender"},
    //Unknown / Unimplimeneted
    // {"Valuable Marble Table", "valuables/03 medium/Valuable Marble Table"},
    // {"Valuable Teeth", "valuables/04 big/Valuable Teeth"},
    // {"Valuable Tray", "valuables/04 big/Valuable Tray"},
    // {"Valuable Plane", "valuables/02 small/Valuable Plane"}




};


    public static readonly Dictionary<string, string> EnemyPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Beamer", "Clown" },
            { "Duck", "Apex Predator" },
            { "Robe", "Robe" },
            { "Bowtie", "Bowtie" },
            { "Floater", "Mentalist" },
            { "Gnome", "Gnomes" },
            { "Hunter", "Huntsman" },
            { "Tumbler", "Chef" },
            { "Thin Man", "Shadow Child" },
            { "Slow Mouth", "Spewer" },
            { "Upscream", "Upscream" },
            { "Hidden", "Hidden" },
            { "Bang", "Bangers" },
            { "Head", "Headman" },
            { "Runner", "Reaper" },
            { "Slow Walker", "Trudge"}
        };
}