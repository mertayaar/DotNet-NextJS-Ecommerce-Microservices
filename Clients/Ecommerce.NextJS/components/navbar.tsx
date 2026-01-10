"use client";



import Link from "next/link";
import { useRouter } from "next/navigation";
import { ShoppingCart, Search, Menu, User, LogOut, Loader2, ShoppingBag, Grid2X2, Info, Mail } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { useAuth } from "@/contexts/AuthContext";
import { useCart } from "@/contexts/CartContext";

export function Navbar() {
    const router = useRouter();
    const { user, isAuthenticated, isLoading, logout } = useAuth();
    const { itemCount } = useCart();

    
    const handleLogout = async () => {
        await logout();
        router.push("/");
        router.refresh();
    };

    
    const getUserInitials = () => {
        if (user?.name) {
            return user.name
                .split(" ")
                .map((n) => n[0])
                .join("")
                .toUpperCase()
                .slice(0, 2);
        }
        return "U";
    };

    return (
        <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
            <div className="container flex h-16 items-center justify-between">
                {}
                <div className="flex md:hidden">
                    <Sheet>
                        <SheetTrigger asChild>
                            <Button variant="ghost" size="icon" className="-ml-2">
                                <Menu className="h-5 w-5" />
                                <span className="sr-only">Toggle menu</span>
                            </Button>
                        </SheetTrigger>
                        <SheetContent side="left" className="w-[300px] sm:w-[400px] flex flex-col">
                            <div className="flex items-center gap-2 mb-8 mt-4 pl-4">
                                <span className="text-xl font-bold tracking-widest text-foreground uppercase">
                                    Minimal Store
                                </span>
                            </div>
                            <nav className="flex flex-col gap-2">
                                <Link
                                    href="/"
                                    className="flex items-center gap-3 px-4 py-3 text-lg font-medium rounded-md hover:bg-muted transition-colors"
                                >
                                    <ShoppingBag className="h-5 w-5" />
                                    Home
                                </Link>
                                <Link
                                    href="/products"
                                    className="flex items-center gap-3 px-4 py-3 text-lg font-medium rounded-md hover:bg-muted transition-colors"
                                >
                                    <Grid2X2 className="h-5 w-5" />
                                    Shop
                                </Link>
                                <Link
                                    href="/about"
                                    className="flex items-center gap-3 px-4 py-3 text-lg font-medium rounded-md hover:bg-muted transition-colors"
                                >
                                    <Info className="h-5 w-5" />
                                    About
                                </Link>
                                <Link
                                    href="/contact"
                                    className="flex items-center gap-3 px-4 py-3 text-lg font-medium rounded-md hover:bg-muted transition-colors"
                                >
                                    <Mail className="h-5 w-5" />
                                    Contact
                                </Link>
                            </nav>

                            <div className="mt-4 pt-6 pb-6 px-2 border-t">
                                {isAuthenticated ? (
                                    <div className="flex flex-col gap-2">
                                        <Link
                                            href="/profile"
                                            className="flex items-center gap-3 px-4 py-3 text-lg font-medium rounded-md hover:bg-muted transition-colors"
                                        >
                                            <User className="h-5 w-5" />
                                            Profile
                                        </Link>
                                        <button
                                            onClick={handleLogout}
                                            className="flex items-center gap-3 px-4 py-3 text-lg font-medium rounded-md hover:bg-destructive/10 text-destructive transition-colors w-full text-left"
                                        >
                                            <LogOut className="h-5 w-5" />
                                            Sign Out
                                        </button>
                                    </div>
                                ) : (
                                    <div className="flex flex-col gap-3">
                                        <Button asChild className="w-full justify-start" variant="default" size="lg">
                                            <Link href="/login">
                                                Sign In
                                            </Link>
                                        </Button>
                                        <Button asChild className="w-full justify-start" variant="outline" size="lg">
                                            <Link href="/register">
                                                Register
                                            </Link>
                                        </Button>
                                    </div>
                                )}
                            </div>
                        </SheetContent>
                    </Sheet>
                </div>

                {}
                <nav className="hidden md:flex items-center space-x-6 text-sm font-medium">
                    <Link
                        href="/"
                        className="transition-colors hover:text-foreground/80 text-foreground/60"
                    >
                        Home
                    </Link>
                    <Link
                        href="/products"
                        className="transition-colors hover:text-foreground/80 text-foreground/60"
                    >
                        Shop
                    </Link>
                    <Link
                        href="/about"
                        className="transition-colors hover:text-foreground/80 text-foreground/60"
                    >
                        About
                    </Link>
                    <Link
                        href="/contact"
                        className="transition-colors hover:text-foreground/80 text-foreground/60"
                    >
                        Contact
                    </Link>
                </nav>

                {}
                <div className="absolute left-1/2 transform -translate-x-1/2">
                    <Link href="/" className="flex items-center">
                        <span className="text-xl font-bold tracking-widest text-foreground uppercase">
                            Minimal Store
                        </span>
                    </Link>
                </div>

                {}
                <div className="flex items-center space-x-2">
                    <Button
                        variant="ghost"
                        size="icon"
                        aria-label="Search"
                        className="hidden md:flex"
                    >
                        <Search className="h-5 w-5" />
                    </Button>

                    <Button variant="ghost" size="icon" aria-label="Cart" asChild className="relative">
                        <Link href="/cart">
                            <ShoppingCart className="h-5 w-5" />
                            {itemCount > 0 && (
                                <span className="absolute -top-1 -right-1 h-5 w-5 rounded-full bg-gradient-to-r from-purple-600 to-pink-600 flex items-center justify-center text-[10px] font-bold text-white shadow-lg">
                                    {itemCount > 99 ? '99+' : itemCount}
                                </span>
                            )}
                        </Link>
                    </Button>

                    {}
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="icon" className="rounded-full">
                                {isLoading ? (
                                    <Loader2 className="h-5 w-5 animate-spin" />
                                ) : isAuthenticated ? (
                                    <Avatar className="h-8 w-8">
                                        <AvatarFallback className="text-xs">
                                            {getUserInitials()}
                                        </AvatarFallback>
                                    </Avatar>
                                ) : (
                                    <User className="h-5 w-5" />
                                )}
                                <span className="sr-only">User menu</span>
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                            {isAuthenticated ? (
                                <>
                                    <DropdownMenuLabel>
                                        {user?.name || user?.email || "My Account"}
                                    </DropdownMenuLabel>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem asChild>
                                        <Link href="/profile">Profile</Link>
                                    </DropdownMenuItem>
                                    <DropdownMenuItem asChild>
                                        <Link href="/cart">My Cart</Link>
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem
                                        onClick={handleLogout}
                                        className="text-destructive focus:text-destructive"
                                    >
                                        <LogOut className="mr-2 h-4 w-4" />
                                        Sign Out
                                    </DropdownMenuItem>
                                </>
                            ) : (
                                <>
                                    <DropdownMenuLabel>My Account</DropdownMenuLabel>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem asChild>
                                        <Link href="/login">Sign In</Link>
                                    </DropdownMenuItem>
                                    <DropdownMenuItem asChild>
                                        <Link href="/register">Register</Link>
                                    </DropdownMenuItem>
                                </>
                            )}
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            </div>
        </header>
    );
}
