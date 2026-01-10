import Link from "next/link"
import { Package2, LayoutDashboard, Settings, ShoppingBag, ListOrdered, Users } from "lucide-react"

export default function AdminLayout({
    children,
}: {
    children: React.ReactNode
}) {
    return (
        <div className="flex min-h-[calc(100vh-4rem)]">
            {}
            <aside className="w-64 border-r bg-muted/40 hidden md:block">
                <div className="flex h-full max-h-screen flex-col gap-2">
                    <div className="flex-1 py-10 px-6">
                        <nav className="grid items-start px-2 text-sm font-medium lg:px-4 space-y-2">
                            <Link
                                href="/admin"
                                className="flex items-center gap-3 rounded-lg px-3 py-2 text-muted-foreground transition-all hover:text-primary hover:bg-muted"
                            >
                                <LayoutDashboard className="h-4 w-4" />
                                Dashboard
                            </Link>
                            <Link
                                href="/admin/products"
                                className="flex items-center gap-3 rounded-lg px-3 py-2 text-muted-foreground transition-all hover:text-primary hover:bg-muted"
                            >
                                <ShoppingBag className="h-4 w-4" />
                                Products
                            </Link>
                            <Link
                                href="/admin/categories"
                                className="flex items-center gap-3 rounded-lg px-3 py-2 text-muted-foreground transition-all hover:text-primary hover:bg-muted"
                            >
                                <ListOrdered className="h-4 w-4" />
                                Categories
                            </Link>
                            <Link
                                href="/admin/users"
                                className="flex items-center gap-3 rounded-lg px-3 py-2 text-muted-foreground transition-all hover:text-primary hover:bg-muted"
                            >
                                <Users className="h-4 w-4" />
                                Customers
                            </Link>
                            <Link
                                href="/admin/settings"
                                className="flex items-center gap-3 rounded-lg px-3 py-2 text-muted-foreground transition-all hover:text-primary hover:bg-muted"
                            >
                                <Settings className="h-4 w-4" />
                                Settings
                            </Link>
                        </nav>
                    </div>
                </div>
            </aside>

            {}
            <main className="flex-1 p-8 overflow-y-auto">
                {children}
            </main>
        </div>
    )
}
